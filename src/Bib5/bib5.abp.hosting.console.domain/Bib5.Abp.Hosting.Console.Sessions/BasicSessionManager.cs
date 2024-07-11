using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.IdentityModel;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Bib5.Abp.Hosting.Console.Sessions;

/// <summary>
/// 会话信息管理,只保存一次会话
/// </summary>
public abstract class BasicSessionManager : IBasicSessionManager, ISingletonDependency
{
	protected IAbpLazyServiceProvider LazyServiceProvider { get; }

	protected ISessionRepository SessionRepository => LazyServiceProvider.LazyGetRequiredService<ISessionRepository>();

	protected ILoginProvider LoginProvider => LazyServiceProvider.LazyGetRequiredService<ILoginProvider>();

	protected IJsonSerializer JsonSerializer => LazyServiceProvider.LazyGetRequiredService<IJsonSerializer>();

	protected IOptionsMonitor<AbpIdentityClientOptions> IdentityClientOptions => LazyServiceProvider.LazyGetRequiredService<IOptionsMonitor<AbpIdentityClientOptions>>();

	protected ICurrentTenant CurrentTenant => (ICurrentTenant)(object)LazyServiceProvider.LazyGetRequiredService<CurrentTenant>();

	protected IDistributedCache<SessionCacheItem> Cache => LazyServiceProvider.LazyGetRequiredService<IDistributedCache<SessionCacheItem>>();

	protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();

	protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

	protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>((Func<IServiceProvider, object>)((IServiceProvider provider) => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance));

	public BasicSessionManager(IAbpLazyServiceProvider lazyServiceProvider)
	{
		LazyServiceProvider = lazyServiceProvider;
	}

	/// <summary>
	/// 设置会话信息
	/// </summary>
	/// <param name="session"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
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
		return curSession;
	}

	/// <summary>
	/// 获取会话信息,如果回话接近过期将会被刷新(阈值:过期前1分钟)
	/// </summary>
	/// <param name="sessionKey">会话Key,用于区分会话属于哪个远程服务</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task<Session?> GetAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		IDistributedCache<SessionCacheItem> cache = Cache;
		string text = SessionCacheItem.CalculateKey(sessionKey);
		CancellationToken cancellationToken2 = cancellationToken;
		SessionCacheItem sessionCache = await ((IDistributedCache<SessionCacheItem, string>)(object)cache).GetAsync(text, (bool?)null, false, cancellationToken2);
		Session curSession;
		if (sessionCache == null)
		{
			curSession = await ((IReadOnlyBasicRepository<Session, string>)SessionRepository).FindAsync(sessionKey, false, cancellationToken);
			if (curSession != null)
			{
				IDistributedCache<SessionCacheItem> cache2 = Cache;
				string text2 = SessionCacheItem.CalculateKey(sessionKey);
				SessionCacheItem sessionCacheItem = ObjectMapper.Map<Session, SessionCacheItem>(curSession);
				cancellationToken2 = cancellationToken;
				await ((IDistributedCache<SessionCacheItem, string>)(object)cache2).SetAsync(text2, sessionCacheItem, (DistributedCacheEntryOptions)null, (bool?)null, false, cancellationToken2);
			}
		}
		else
		{
			curSession = ObjectMapper.Map<SessionCacheItem, Session>(sessionCache);
		}
		if (curSession != null && curSession.ExpiresIn() > 0 && curSession.ExpiresIn() < 60)
		{
			await SetAsync(sessionKey, await LoginProvider.LoginAsync(sessionKey, curSession.RefreshToken, cancellationToken), cancellationToken);
		}
		return curSession;
	}

	protected async Task<Session> SetAsync(string sessionKey, Session sessionWithoutKey, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration(CurrentTenant, sessionKey);
		Session session = new Session(sessionKey, config.Authority)
		{
			AccessToken = sessionWithoutKey.AccessToken,
			ExpiresAt = sessionWithoutKey.ExpiresAt,
			RefreshToken = sessionWithoutKey.RefreshToken
		};
		return await SetAsync(session, cancellationToken);
	}

	/// <summary>
	/// 是否有效回话
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task<bool> IsVaildAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		return !(await GetAsync(sessionKey, cancellationToken)).IsExpires();
	}

	/// <summary>
	/// 获取会话状态(带缓存)
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
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

	/// <summary>
	/// 退出
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public virtual async Task LogoutAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration(CurrentTenant, sessionKey);
		await SetAsync(new Session(sessionKey, config.Authority), cancellationToken);
	}

	/// <summary>
	/// 重新挂载会话数据到缓存
	/// </summary>
	/// <param name="sessionKey"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public virtual async Task ReloadAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session session = await ((IReadOnlyBasicRepository<Session, string>)SessionRepository).GetAsync(sessionKey, true, cancellationToken);
		await ((IDistributedCache<SessionCacheItem, string>)(object)Cache).SetAsync(SessionCacheItem.CalculateKey(((Entity<string>)(object)session).Id), ObjectMapper.Map<Session, SessionCacheItem>(session), (DistributedCacheEntryOptions)null, (bool?)null, false, cancellationToken);
		ILogger logger = Logger;
		bool flag = await IsVaildAsync(sessionKey, cancellationToken);
		logger.LogDebug("SessionManager 刷新 IsVaild: {0}", flag);
	}
}
