using System.Threading.Tasks;
using Bib5.Abp.Sessions;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.Authentication;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.IdentityModel;

namespace Volo.Abp.Security.Claims;

[Dependency(ReplaceServices = true)]
public class SimpleAccessTokenProviderIdentityModelRemoteServiceHttpClientAuthenticator : IdentityModelRemoteServiceHttpClientAuthenticator
{
	protected IBasicSessionManager SessionManager { get; }

	protected SessionOptions SessionOptions { get; }

	public SimpleAccessTokenProviderIdentityModelRemoteServiceHttpClientAuthenticator(IIdentityModelAuthenticationService identityModelAuthenticationService, IOptions<SessionOptions> sessionOptions, IBasicSessionManager sessionManager)
		: base(identityModelAuthenticationService)
	{
		SessionOptions = sessionOptions.Value;
		SessionManager = sessionManager;
	}

	public override async Task Authenticate(RemoteServiceHttpClientAuthenticateContext context)
	{
		if (RemoteServiceConfigurationExtensions.GetUseCurrentAccessToken(context.RemoteService) != false)
		{
			string text = await GetAccessTokenOrNullAsync();
			if (text != null)
			{
				AuthorizationHeaderExtensions.SetBearerToken(context.Request, text);
			}
		}
	}

	protected virtual async Task<string> GetAccessTokenOrNullAsync()
	{
		Session session = await SessionManager.GetAsync(SessionOptions.Session.SessionKey);
		if (session.IsExpires())
		{
			return null;
		}
		return session.AccessToken;
	}
}
