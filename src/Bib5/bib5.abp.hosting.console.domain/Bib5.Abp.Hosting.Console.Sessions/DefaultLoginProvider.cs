using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.IdentityModel;
using Volo.Abp.Randoms;

namespace Bib5.Abp.Hosting.Console.Sessions;

public class DefaultLoginProvider : DomainService, ILoginProvider
{
	protected IHttpClientFactory HttpClientFactory => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>();

	protected IDistributedCache<IdentityModelDiscoveryDocumentCacheItem> DiscoveryDocumentCache => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IDistributedCache<IdentityModelDiscoveryDocumentCacheItem>>();

	protected IAbpHostEnvironment AbpHostEnvironment => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IAbpHostEnvironment>();

	protected IStateValidator StateValidator => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IStateValidator>();

	protected IRandomDataGenerator RandomDataGenerator => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IRandomDataGenerator>();

	protected IOptionsMonitor<AbpIdentityClientOptions> IdentityClientOptions => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IOptionsMonitor<AbpIdentityClientOptions>>();

	public async Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new Exception("Client " + sessionKey + "未配置");
		config.UserName = userName;
		config.UserPassword = password;
		using HttpClient httpClient = HttpClientFactory.CreateClient(config.Authority);
		HttpMessageInvoker httpMessageInvoker = httpClient;
		TokenResponse response = await HttpClientTokenRequestExtensions.RequestPasswordTokenAsync(httpMessageInvoker, await CreatePasswordTokenRequestAsync(config, cancellationToken), cancellationToken);
		if (((ProtocolResponse)response).HttpStatusCode != HttpStatusCode.OK)
		{
			throw new InvalidOperationException(response.ErrorDescription);
		}
		return new Session
		{
			AccessToken = response.AccessToken,
			RefreshToken = response.RefreshToken,
			ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
		};
	}

	public async Task<Session> LoginAsync(string sessionKey, string refreshToken, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new Exception("Client " + sessionKey + "未配置");
		using HttpClient httpClient = HttpClientFactory.CreateClient(config.Authority);
		HttpMessageInvoker httpMessageInvoker = httpClient;
		TokenResponse response = await HttpClientTokenRequestExtensions.RequestRefreshTokenAsync(httpMessageInvoker, await CreateRefreshTokenRequestAsync(config, refreshToken, cancellationToken), cancellationToken);
		if (((ProtocolResponse)response).HttpStatusCode != HttpStatusCode.OK)
		{
			throw new InvalidOperationException(response.ErrorDescription);
		}
		return new Session
		{
			AccessToken = response.AccessToken,
			RefreshToken = response.RefreshToken,
			ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
		};
	}

	public async Task<string> LoginRedirectAsync(string sessionKey, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration(this.CurrentTenant, sessionKey) ?? throw new Exception("Client " + sessionKey + "未配置");
		string state = RandomDataGenerator.GetString(32);
		await StateValidator.AddStateAsync(config.Authority, state, cancellationToken);
		return state;
	}

	public async Task<Session> LoginRedirectCallbackAsync(string sessionKey, string code, string state, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new Exception("Client " + sessionKey + "未配置");
		if (!(await StateValidator.ValidateAsync(config.Authority, state, cancellationToken)))
		{
			throw new BusinessException("Bib5.Hosting.Console.Session:000002", (string)null, (string)null, (Exception)null, LogLevel.Warning);
		}
		using HttpClient httpClient = HttpClientFactory.CreateClient(config.Authority);
		HttpMessageInvoker httpMessageInvoker = httpClient;
		TokenResponse response = await HttpClientTokenRequestExtensions.RequestAuthorizationCodeTokenAsync(httpMessageInvoker, await CreateAuthorizationCodeTokenRequestAsync(config, code, cancellationToken), cancellationToken);
		if (((ProtocolResponse)response).HttpStatusCode != HttpStatusCode.OK)
		{
			throw new InvalidOperationException(response.ErrorDescription);
		}
		return new Session
		{
			AccessToken = response.AccessToken,
			RefreshToken = response.RefreshToken,
			ExpiresAt = DateTime.Now.AddSeconds(response.ExpiresIn)
		};
	}

	protected virtual string CalculateDiscoveryDocumentCacheKey(IdentityClientConfiguration config)
	{
		return IdentityModelDiscoveryDocumentCacheItem.CalculateCacheKey(config);
	}

	protected virtual async Task<IdentityModelDiscoveryDocumentCacheItem> GetDiscoveryResponseAsync(IdentityClientConfiguration config, CancellationToken cancellationToken)
	{
		string tokenEndpointUrlCacheKey = CalculateDiscoveryDocumentCacheKey(config);
		IDistributedCache<IdentityModelDiscoveryDocumentCacheItem> discoveryDocumentCache = DiscoveryDocumentCache;
		CancellationToken cancellationToken2 = cancellationToken;
		IdentityModelDiscoveryDocumentCacheItem discoveryDocumentCacheItem = await ((IDistributedCache<IdentityModelDiscoveryDocumentCacheItem, string>)(object)discoveryDocumentCache).GetAsync(tokenEndpointUrlCacheKey, (bool?)null, false, cancellationToken2);
		if (discoveryDocumentCacheItem == null)
		{
			DiscoveryDocumentResponse discoveryResponse;
			using (HttpClient httpClient = HttpClientFactory.CreateClient(config.Authority))
			{
				DiscoveryDocumentRequest val = new DiscoveryDocumentRequest
				{
					Address = config.Authority
				};
				val.Policy.RequireHttps = config.RequireHttps;
				val.Policy.ValidateIssuerName = config.ValidateIssuerName;
				val.Policy.ValidateEndpoints = config.ValidateEndpoints;
				DiscoveryDocumentRequest request = val;
				discoveryResponse = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync((HttpMessageInvoker)httpClient, request, cancellationToken);
			}
			if (((ProtocolResponse)discoveryResponse).IsError)
			{
				throw new AbpException($"Could not retrieve the OpenId Connect discovery document! ErrorType: {((ProtocolResponse)discoveryResponse).ErrorType}. Error: {((ProtocolResponse)discoveryResponse).Error}");
			}
			discoveryDocumentCacheItem = new IdentityModelDiscoveryDocumentCacheItem(discoveryResponse.TokenEndpoint, discoveryResponse.DeviceAuthorizationEndpoint);
			IDistributedCache<IdentityModelDiscoveryDocumentCacheItem> discoveryDocumentCache2 = DiscoveryDocumentCache;
			IdentityModelDiscoveryDocumentCacheItem obj = discoveryDocumentCacheItem;
			DistributedCacheEntryOptions obj2 = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = (AbpHostEnvironmentExtensions.IsDevelopment(AbpHostEnvironment) ? TimeSpan.FromSeconds(5.0) : TimeSpan.FromSeconds(config.CacheAbsoluteExpiration))
			};
			cancellationToken2 = cancellationToken;
			await ((IDistributedCache<IdentityModelDiscoveryDocumentCacheItem, string>)(object)discoveryDocumentCache2).SetAsync(tokenEndpointUrlCacheKey, obj, obj2, (bool?)null, false, cancellationToken2);
		}
		return discoveryDocumentCacheItem;
	}

	protected virtual async Task<PasswordTokenRequest> CreatePasswordTokenRequestAsync(IdentityClientConfiguration config, CancellationToken cancellationToken)
	{
		IdentityModelDiscoveryDocumentCacheItem discoveryResponse = await GetDiscoveryResponseAsync(config, cancellationToken);
		return new PasswordTokenRequest
		{
			Address = discoveryResponse.TokenEndpoint,
			Scope = config.Scope,
			ClientId = config.ClientId,
			ClientSecret = config.ClientSecret,
			UserName = config.UserName,
			Password = config.UserPassword
		};
	}

	protected virtual async Task<RefreshTokenRequest> CreateRefreshTokenRequestAsync(IdentityClientConfiguration config, string refreshToken, CancellationToken cancellationToken)
	{
		IdentityModelDiscoveryDocumentCacheItem discoveryResponse = await GetDiscoveryResponseAsync(config, cancellationToken);
		return new RefreshTokenRequest
		{
			Address = discoveryResponse.TokenEndpoint,
			Scope = config.Scope,
			ClientId = config.ClientId,
			ClientSecret = config.ClientSecret,
			RefreshToken = refreshToken
		};
	}

	protected virtual async Task<AuthorizationCodeTokenRequest> CreateAuthorizationCodeTokenRequestAsync(IdentityClientConfiguration config, string code, CancellationToken cancellationToken)
	{
		IdentityModelDiscoveryDocumentCacheItem discoveryResponse = await GetDiscoveryResponseAsync(config, cancellationToken);
		return new AuthorizationCodeTokenRequest
		{
			Address = discoveryResponse.TokenEndpoint,
			ClientId = config.ClientId,
			RedirectUri = ((Dictionary<string, string>)(object)config)["RedirectUri"],
			Code = code
		};
	}
}
