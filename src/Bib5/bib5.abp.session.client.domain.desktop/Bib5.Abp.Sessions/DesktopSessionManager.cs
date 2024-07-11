using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace Bib5.Abp.Sessions;

[ExposeServices(new Type[]
{
	typeof(IDesktopSessionManager),
	typeof(IBasicSessionManager)
})]
public class DesktopSessionManager : BasicSessionManager, IDesktopSessionManager, IBasicSessionManager
{
	protected ISessionAppService SessionAppService => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<ISessionAppService>();

	public async Task<string> LoginRedirectAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		return await base.LoginProvider.LoginRedirectAsync(sessionKey, cancellationToken);
	}

	public override async Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session session = await base.LoginAsync(sessionKey, userName, password, cancellationToken);
		if (base.SessionOptions.CurrentValue.Session.EnableLocalSynchronization)
		{
			await SessionAppService.LoginNotifyAsync(cancellationToken);
		}
		return session;
	}

	public override async Task LogoutAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken))
	{
		await base.LogoutAsync(sessionKey, cancellationToken);
		if (base.SessionOptions.CurrentValue.Session.EnableLocalSynchronization)
		{
			await SessionAppService.LogoutNotifyAsync(cancellationToken);
		}
	}
}
