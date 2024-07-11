using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.IdentityModel;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Bib5.Abp.Sessions;

public abstract class BasicSessionManager : DomainService, IBasicSessionManager
{
	protected ISessionRepository SessionRepository => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<ISessionRepository>();

	protected ILoginProvider LoginProvider => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<ILoginProvider>();

	protected IJsonSerializer JsonSerializer => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IJsonSerializer>();

	protected IOptionsMonitor<AbpIdentityClientOptions> IdentityClientOptions => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IOptionsMonitor<AbpIdentityClientOptions>>();

	protected IDistributedCache<SessionCacheItem> Cache => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IDistributedCache<SessionCacheItem>>();

	protected IObjectMapper ObjectMapper => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();

	protected IOptionsMonitor<SessionOptions> SessionOptions => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IOptionsMonitor<SessionOptions>>();

	public ConcurrentDictionary<string, ChangeTokenInfo> SessionTokenLookup => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IObjectAccessor<ConcurrentDictionary<string, ChangeTokenInfo>>>().Value;

	[UnitOfWork]
	public async Task<Session> SetAsync(Session session, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session curSession = await ((IReadOnlyBasicRepository<Session, string>)SessionRepository).FindAsync(((Entity<string>)(object)session).Id, false, cancellationToken);
		if (curSession != null)
		{
			curSession.AccessToken = session.AccessToken;
			curSession.RefreshToken = session.RefreshToken;
			curSession.ExpiresAt = session.ExpiresAt;
			await ((IBasicRepository<Session>)SessionRepository).UpdateAsync(curSession, false, cancellationToken);
		}
		else
		{
			curSession = await ((IBasicRepository<Session>)SessionRepository).InsertAsync(session, false, cancellationToken);
		}
		await ((IDistributedCache<SessionCacheItem, string>)(object)Cache).SetAsync(SessionCacheItem.CalculateKey(((Entity<string>)(object)session).Id), ObjectMapper.Map<Session, SessionCacheItem>(session), (DistributedCacheEntryOptions)null, (bool?)null, false, cancellationToken);
		OnAccessTokenChanged(((Entity<string>)(object)session).Id);
		return curSession;
	}

	public async Task<Session?> GetAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		IDistributedCache<SessionCacheItem> cache = Cache;
		string text = SessionCacheItem.CalculateKey(sessionKey);
		CancellationToken cancellationToken2 = cancellationToken;
		SessionCacheItem sessionCacheItem = await ((IDistributedCache<SessionCacheItem, string>)(object)cache).GetAsync(text, (bool?)null, false, cancellationToken2);
		Session curSession;
		if (sessionCacheItem == null)
		{
			curSession = await ((IReadOnlyBasicRepository<Session, string>)SessionRepository).FindAsync(sessionKey, false, cancellationToken);
			if (curSession != null)
			{
				IDistributedCache<SessionCacheItem> cache2 = Cache;
				string text2 = SessionCacheItem.CalculateKey(sessionKey);
				SessionCacheItem sessionCacheItem2 = ObjectMapper.Map<Session, SessionCacheItem>(curSession);
				cancellationToken2 = cancellationToken;
				await ((IDistributedCache<SessionCacheItem, string>)(object)cache2).SetAsync(text2, sessionCacheItem2, (DistributedCacheEntryOptions)null, (bool?)null, false, cancellationToken2);
			}
		}
		else
		{
			curSession = ObjectMapper.Map<SessionCacheItem, Session>(sessionCacheItem);
		}
		if (curSession != null && curSession.ExpiresIn() > 0 && curSession.ExpiresIn() < 60)
		{
			await SetAsync(sessionKey, await LoginProvider.LoginAsync(sessionKey, curSession.RefreshToken, cancellationToken), cancellationToken);
		}
		return curSession;
	}

	protected async Task<Session> SetAsync(string sessionKey, Session sessionWithoutKey, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration val = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new AbpException("未找到IdentityClient[" + sessionKey + "]");
		Session session = new Session(sessionKey, val.Authority)
		{
			AccessToken = sessionWithoutKey.AccessToken,
			ExpiresAt = sessionWithoutKey.ExpiresAt,
			RefreshToken = sessionWithoutKey.RefreshToken
		};
		return await SetAsync(session, cancellationToken);
	}

	public async Task<bool> IsValidAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		return !(await GetAsync(sessionKey, cancellationToken)).IsExpires();
	}

	public async Task<SessionStatus> GetSessionStatusAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session session = await GetAsync(sessionKey, cancellationToken);
		if (!session.ExpiresAt.HasValue)
		{
			return SessionStatus.NotLoggedIn;
		}
		if (session.IsExpires())
		{
			return SessionStatus.Expires;
		}
		return SessionStatus.Vaild;
	}

	public virtual async Task LogoutAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		IdentityClientConfiguration val = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new AbpException("未找到IdentityClient[" + sessionKey + "]");
		await SetAsync(new Session(sessionKey, val.Authority), cancellationToken);
	}

	public virtual async Task ReloadAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session session = await ((IReadOnlyBasicRepository<Session, string>)SessionRepository).GetAsync(sessionKey, true, cancellationToken);
		await ((IDistributedCache<SessionCacheItem, string>)(object)Cache).SetAsync(SessionCacheItem.CalculateKey(((Entity<string>)(object)session).Id), ObjectMapper.Map<Session, SessionCacheItem>(session), (DistributedCacheEntryOptions)null, (bool?)null, false, cancellationToken);
		OnAccessTokenChanged(sessionKey);
		ILogger logger = (this).Logger;
		bool flag = await IsValidAsync(sessionKey, cancellationToken);
		logger.LogDebug("SessionManager 刷新 IsValid: {0}", flag);
	}

	public virtual async Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken = default(CancellationToken))
	{
		return await SetAsync(sessionKey, await LoginProvider.LoginAsync(sessionKey, userName, password, cancellationToken), cancellationToken);
	}

	private ChangeTokenInfo GetOrAddChangeTokenInfo(string sessionKey)
	{
		return AbpDictionaryExtensions.GetOrAdd<string, ChangeTokenInfo>(SessionTokenLookup, sessionKey, (Func<ChangeTokenInfo>)delegate
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationChangeToken changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
			return new ChangeTokenInfo(cancellationTokenSource, changeToken);
		});
	}

	protected void OnAccessTokenChanged(string sessionKey)
	{
		if (SessionTokenLookup.TryRemove(sessionKey, out var value))
		{
			Task.Run((Action)value.TokenSource.Cancel);
		}
	}

	public IChangeToken GetAccessTokenChangeToken(string sessionKey)
	{
		return GetOrAddChangeTokenInfo(sessionKey).ChangeToken;
	}
}
