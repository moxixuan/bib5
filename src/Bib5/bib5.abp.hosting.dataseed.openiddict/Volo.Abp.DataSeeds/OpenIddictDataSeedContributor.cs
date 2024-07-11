using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUglify.Helpers;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DataSeeds.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Volo.Abp.DataSeeds;

public class OpenIddictDataSeedContributor : IDataSeedContributor, ITransientDependency
{
	public IAbpLazyServiceProvider LazyServiceProvider { get; }

	protected IAbpApplicationManager ApplicationManager => LazyServiceProvider.LazyGetRequiredService<IAbpApplicationManager>();

	protected IOpenIddictScopeManager ScopeManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictScopeManager>();

	protected OpenIddictConfig DataSeed => LazyServiceProvider.LazyGetRequiredService<IOptions<OpenIddictOptions>>().Value.DataSeed;

	protected IStringLocalizer<OpenIddictResponse> L => LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<OpenIddictResponse>>();

	protected IPermissionDataSeeder PermissionDataSeeder => LazyServiceProvider.LazyGetRequiredService<IPermissionDataSeeder>();

	public OpenIddictDataSeedContributor(IAbpLazyServiceProvider lazyServiceProvider):base()
	{
		LazyServiceProvider = lazyServiceProvider;
	}

	[UnitOfWork]
	public async Task SeedAsync(DataSeedContext context)
	{
		await CreateScopesAsync();
		await CreateApplicationsAsync();
	}

	protected async Task CreateScopesAsync()
	{
		ScopeConfig[] scopes = DataSeed.Scopes;
		foreach (ScopeConfig scopeConfig in scopes)
		{
			if (await ScopeManager.FindByNameAsync(scopeConfig.Name, default(CancellationToken)) == null)
			{
				OpenIddictScopeDescriptor scopeDescriptor = new OpenIddictScopeDescriptor
				{
					Name = scopeConfig.Name,
					DisplayName = scopeConfig.DisplayName,
					Description = scopeConfig.Description
				};
				NUglifyExtensions.ForEach<string>((IEnumerable<string>)scopeConfig.Resources, (Action<string>)delegate(string r)
				{
					scopeDescriptor.Resources.Add(r);
				});
				await ScopeManager.CreateAsync(scopeDescriptor, default(CancellationToken));
			}
		}
	}

	protected async Task CreateApplicationsAsync()
	{
		ApplicationConfig[] applications = DataSeed.Applications;
		foreach (ApplicationConfig applicationConfig in applications)
		{
			List<string> obj = new List<string> { "scp:address", "scp:email", "scp:phone", "scp:profile", "scp:roles" };
			string clientId = applicationConfig.ClientId;
			string clientType = applicationConfig.ClientType;
			string consentType = applicationConfig.ConsentType;
			string displayName = applicationConfig.DisplayName;
			string clientSecret = applicationConfig.ClientSecret;
			HashSet<string> grantTypes = applicationConfig.GrantTypes;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string item in obj)
			{
				hashSet.Add(item);
			}
			foreach (string scope in applicationConfig.Scopes)
			{
				hashSet.Add(scope);
			}
			await CreateOrUpdateApplicationAsync(clientId, clientType, consentType, displayName, clientSecret, grantTypes, hashSet, applicationConfig.LogoUri, applicationConfig.ClientUri, applicationConfig.RedirectUris, applicationConfig.PostLogoutRedirectUris, applicationConfig.Permissions);
		}
	}

	private async Task CreateOrUpdateApplicationAsync(string clientId, string type, string consentType, string displayName, string? clientSecret, HashSet<string> grantTypes, HashSet<string> scopes, string? logoUri, string? clientUri, HashSet<string>? redirectUris, HashSet<string>? postLogoutRedirectUris, HashSet<string>? permissions = null)
	{
		if (!string.IsNullOrEmpty(clientSecret) && string.Equals(type, "public", StringComparison.OrdinalIgnoreCase))
		{
			throw new BusinessException((string?)L["NoClientSecretCanBeSetForPublicApplications"], (string)null, (string)null, (Exception)null, LogLevel.Warning);
		}
		if (string.IsNullOrEmpty(clientSecret) && string.Equals(type, "confidential", StringComparison.OrdinalIgnoreCase))
		{
			throw new BusinessException((string?)L["TheClientSecretIsRequiredForConfidentialApplications"], (string)null, (string)null, (Exception)null, LogLevel.Warning);
		}
		AbpApplicationDescriptor application = new AbpApplicationDescriptor
		{
			ClientId = clientId,
			ClientType = type,
			ClientSecret = clientSecret,
			ConsentType = consentType,
			DisplayName = displayName,
			LogoUri = logoUri,
			ClientUri = clientUri
		};
		Check.NotNullOrEmpty<string>((ICollection<string>)grantTypes, "grantTypes");
		Check.NotNullOrEmpty<string>((ICollection<string>)scopes, "scopes");
		if (new string[2] { "authorization_code", "implicit" }.All(grantTypes.Contains))
		{
			((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:code id_token");
			if (string.Equals(type, "public", StringComparison.OrdinalIgnoreCase))
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:code id_token token");
				((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:code token");
			}
		}
		if (!AbpCollectionExtensions.IsNullOrEmpty<string>((ICollection<string>)redirectUris) || !AbpCollectionExtensions.IsNullOrEmpty<string>((ICollection<string>)postLogoutRedirectUris))
		{
			((OpenIddictApplicationDescriptor)application).Permissions.Add("ept:logout");
		}
		_ = new string[6] { "implicit", "password", "authorization_code", "client_credentials", "urn:ietf:params:oauth:grant-type:device_code", "refresh_token" };
		foreach (string grantType in grantTypes)
		{
			if (grantType == "authorization_code")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("gt:authorization_code");
				((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:code");
			}
			if (grantType == "authorization_code" || grantType == "implicit")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("ept:authorization");
			}
			switch (grantType)
			{
			case "authorization_code":
			case "client_credentials":
			case "password":
			case "refresh_token":
			case "urn:ietf:params:oauth:grant-type:device_code":
				((OpenIddictApplicationDescriptor)application).Permissions.Add("ept:token");
				((OpenIddictApplicationDescriptor)application).Permissions.Add("ept:revocation");
				((OpenIddictApplicationDescriptor)application).Permissions.Add("ept:introspection");
				break;
			}
			if (grantType == "client_credentials")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("gt:client_credentials");
			}
			if (grantType == "implicit")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("gt:implicit");
			}
			if (grantType == "password")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("gt:password");
			}
			if (grantType == "refresh_token")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("gt:refresh_token");
			}
			if (grantType == "urn:ietf:params:oauth:grant-type:device_code")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("gt:urn:ietf:params:oauth:grant-type:device_code");
				((OpenIddictApplicationDescriptor)application).Permissions.Add("ept:device");
			}
			if (grantType == "implicit")
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:id_token");
				if (string.Equals(type, "public", StringComparison.OrdinalIgnoreCase))
				{
					((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:id_token token");
					((OpenIddictApplicationDescriptor)application).Permissions.Add("rst:token");
				}
			}
		}
		string[] source = new string[5] { "scp:address", "scp:email", "scp:phone", "scp:profile", "scp:roles" };
		foreach (string scope in scopes)
		{
			if (source.Contains(scope))
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add(scope);
			}
			else
			{
				((OpenIddictApplicationDescriptor)application).Permissions.Add("scp:" + scope);
			}
		}
		if (!AbpCollectionExtensions.IsNullOrEmpty<string>((ICollection<string>)redirectUris))
		{
			foreach (string redirectUri in redirectUris)
			{
				if (redirectUri != null)
				{
					if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out Uri uri2) || !uri2.IsWellFormedOriginalString())
					{
						throw new BusinessException((string?)L["InvalidRedirectUri", new object[1] { redirectUris }], (string)null, (string)null, (Exception)null, LogLevel.Warning);
					}
					if (((OpenIddictApplicationDescriptor)application).RedirectUris.All((Uri x) => x != uri2))
					{
						((OpenIddictApplicationDescriptor)application).RedirectUris.Add(uri2);
					}
				}
			}
		}
		if (!AbpCollectionExtensions.IsNullOrEmpty<string>((ICollection<string>)postLogoutRedirectUris))
		{
			foreach (string postLogoutRedirectUri in postLogoutRedirectUris)
			{
				if (postLogoutRedirectUri != null)
				{
					if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out Uri uri) || !uri.IsWellFormedOriginalString())
					{
						throw new BusinessException((string?)L["InvalidPostLogoutRedirectUri", new object[1] { postLogoutRedirectUris }], (string)null, (string)null, (Exception)null, LogLevel.Warning);
					}
					if (((OpenIddictApplicationDescriptor)application).PostLogoutRedirectUris.All((Uri x) => x != uri))
					{
						((OpenIddictApplicationDescriptor)application).PostLogoutRedirectUris.Add(uri);
					}
				}
			}
		}
		if (permissions != null)
		{
			await PermissionDataSeeder.SeedAsync("C", clientId, (IEnumerable<string>)permissions, (Guid?)null);
		}
		object obj = await ((IOpenIddictApplicationManager)ApplicationManager).FindByClientIdAsync(clientId, default(CancellationToken));
		OpenIddictApplicationModel client = (OpenIddictApplicationModel)((obj is OpenIddictApplicationModel) ? obj : null);
		if (client == null)
		{
			await ((IOpenIddictApplicationManager)ApplicationManager).CreateAsync((OpenIddictApplicationDescriptor)(object)application, default(CancellationToken));
			return;
		}
		if (!HasSameRedirectUris(client, application))
		{
			client.RedirectUris = JsonSerializer.Serialize(((OpenIddictApplicationDescriptor)application).RedirectUris.Select((Uri q) => q.ToString().TrimEnd('/')));
			client.PostLogoutRedirectUris = JsonSerializer.Serialize(((OpenIddictApplicationDescriptor)application).PostLogoutRedirectUris.Select((Uri q) => q.ToString().TrimEnd('/')));
			await ((IOpenIddictApplicationManager)ApplicationManager).UpdateAsync((object)client, default(CancellationToken));
		}
		if (!HasSameScopes(client, application))
		{
			client.Permissions = JsonSerializer.Serialize(((OpenIddictApplicationDescriptor)application).Permissions.Select((string q) => q.ToString()));
			await ((IOpenIddictApplicationManager)ApplicationManager).UpdateAsync((object)client, default(CancellationToken));
		}
	}

	private static bool HasSameRedirectUris(OpenIddictApplicationModel existingClient, AbpApplicationDescriptor application)
	{
		return existingClient.RedirectUris == JsonSerializer.Serialize(((OpenIddictApplicationDescriptor)application).RedirectUris.Select((Uri q) => q.ToString().TrimEnd('/')));
	}

	private static bool HasSameScopes(OpenIddictApplicationModel existingClient, AbpApplicationDescriptor application)
	{
		return existingClient.Permissions == JsonSerializer.Serialize(((OpenIddictApplicationDescriptor)application).Permissions.Select((string q) => q.ToString().TrimEnd('/')));
	}
}
