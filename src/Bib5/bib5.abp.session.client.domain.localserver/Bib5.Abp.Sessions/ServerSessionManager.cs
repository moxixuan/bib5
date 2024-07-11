using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Bib5.Abp.Sessions;

[ExposeServices(new Type[]
{
	typeof(IServerSessionManager),
	typeof(IBasicSessionManager)
})]
public class ServerSessionManager : BasicSessionManager, IServerSessionManager, IBasicSessionManager
{
	protected IUnitOfWorkManager UnitOfWorkManager => (IUnitOfWorkManager)(object)((DomainService)this).LazyServiceProvider.LazyGetRequiredService<UnitOfWorkManager>();

	public async Task<Session?> LoginAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session val = await ((BasicSessionManager)this).GetAsync(sessionKey, cancellationToken);
		if (val != null && val.RefreshToken != null)
		{
			return await (this).SetAsync(sessionKey, await (this).LoginProvider.LoginAsync(sessionKey, val.RefreshToken, cancellationToken), cancellationToken);
		}
		return null;
	}

	public async Task<Session> LoginRedirectCallbackAsync(string sessionKey, string code, string state, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session newSession = await (this).SetAsync(sessionKey, await (this).LoginProvider.LoginRedirectCallbackAsync(sessionKey, code, state, cancellationToken), cancellationToken);
		await UnitOfWorkManager.Current.SaveChangesAsync(cancellationToken);
		return newSession;
	}

	public override async Task LogoutAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		await _003C_003En__0(sessionKey, cancellationToken);
	}

	public async Task ClearAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		await _003C_003En__0(sessionKey, cancellationToken);
	}

	ConcurrentDictionary<string, ChangeTokenInfo> get_SessionTokenLookup()
	{
		return ((BasicSessionManager)this).SessionTokenLookup;
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private Task _003C_003En__0(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ((BasicSessionManager)this).LogoutAsync(sessionKey, cancellationToken);
	}
}
