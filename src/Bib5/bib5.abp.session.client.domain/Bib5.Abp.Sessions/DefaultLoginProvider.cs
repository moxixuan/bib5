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

namespace Bib5.Abp.Sessions;

public class DefaultLoginProvider : DomainService, ILoginProvider
{
	protected IHttpClientFactory HttpClientFactory => (this).LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>();

	protected IDistributedCache<IdentityModelDiscoveryDocumentCacheItem> DiscoveryDocumentCache => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IDistributedCache<IdentityModelDiscoveryDocumentCacheItem>>();

	protected IAbpHostEnvironment AbpHostEnvironment => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IAbpHostEnvironment>();

	protected IStateValidator StateValidator => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IStateValidator>();

	protected IRandomDataGenerator RandomDataGenerator => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IRandomDataGenerator>();

	protected IOptionsMonitor<AbpIdentityClientOptions> IdentityClientOptions => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<IOptionsMonitor<AbpIdentityClientOptions>>();

	public async Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration val = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new AbpException("未找到IdentityClient[" + sessionKey + "]");
		val.UserName = userName;
		val.UserPassword = password;
		using HttpClient httpClient = HttpClientFactory.CreateClient(val.Authority);
		HttpMessageInvoker httpMessageInvoker = httpClient;
		TokenResponse val2 = await HttpClientTokenRequestExtensions.RequestPasswordTokenAsync(httpMessageInvoker, await CreatePasswordTokenRequestAsync(val, cancellationToken), cancellationToken);
		if (((ProtocolResponse)val2).HttpStatusCode != HttpStatusCode.OK)
		{
			throw new InvalidOperationException(val2.ErrorDescription);
		}
		return new Session
		{
			AccessToken = val2.AccessToken,
			RefreshToken = val2.RefreshToken,
			ExpiresAt = DateTime.Now.AddSeconds(val2.ExpiresIn)
		};
	}

	public async Task<Session> LoginAsync(string sessionKey, string refreshToken, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration val = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new AbpException("未找到IdentityClient[" + sessionKey + "]");
		using HttpClient httpClient = HttpClientFactory.CreateClient(val.Authority);
		HttpMessageInvoker httpMessageInvoker = httpClient;
		TokenResponse val2 = await HttpClientTokenRequestExtensions.RequestRefreshTokenAsync(httpMessageInvoker, await CreateRefreshTokenRequestAsync(val, refreshToken, cancellationToken), cancellationToken);
		if (((ProtocolResponse)val2).HttpStatusCode != HttpStatusCode.OK)
		{
			throw new InvalidOperationException(val2.ErrorDescription);
		}
		return new Session
		{
			AccessToken = val2.AccessToken,
			RefreshToken = val2.RefreshToken,
			ExpiresAt = DateTime.Now.AddSeconds(val2.ExpiresIn)
		};
	}

	public async Task<string> LoginRedirectAsync(string sessionKey, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration val = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new AbpException("未找到IdentityClient[" + sessionKey + "]");
		string state = RandomDataGenerator.GetString(32);
		await StateValidator.AddStateAsync(val.Authority, state, cancellationToken);
		return state;
	}

	public async Task<Session> LoginRedirectCallbackAsync(string sessionKey, string code, string state, CancellationToken cancellationToken)
	{
		IdentityClientConfiguration config = IdentityClientOptions.CurrentValue.GetClientConfiguration((this).CurrentTenant, sessionKey) ?? throw new AbpException("未找到IdentityClient[" + sessionKey + "]");
		if (!(await StateValidator.ValidateAsync(config.Authority, state, cancellationToken)))
		{
			throw new BusinessException("Bib5.Session:000002", (string)null, (string)null, (Exception)null, LogLevel.Warning);
		}
		using HttpClient httpClient = HttpClientFactory.CreateClient(config.Authority);
		HttpMessageInvoker httpMessageInvoker = httpClient;
		TokenResponse val = await HttpClientTokenRequestExtensions.RequestAuthorizationCodeTokenAsync(httpMessageInvoker, await CreateAuthorizationCodeTokenRequestAsync(config, code, cancellationToken), cancellationToken);
		if (((ProtocolResponse)val).HttpStatusCode != HttpStatusCode.OK)
		{
			throw new InvalidOperationException(val.ErrorDescription);
		}
		return new Session
		{
			AccessToken = val.AccessToken,
			RefreshToken = val.RefreshToken,
			ExpiresAt = DateTime.Now.AddSeconds(val.ExpiresIn)
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
			DiscoveryDocumentResponse val3;
			using (HttpClient httpClient = HttpClientFactory.CreateClient(config.Authority))
			{
				DiscoveryDocumentRequest val = new DiscoveryDocumentRequest
				{
					Address = config.Authority
				};
				val.Policy.RequireHttps = config.RequireHttps;
				val.Policy.ValidateIssuerName = config.ValidateIssuerName;
				val.Policy.ValidateEndpoints = config.ValidateEndpoints;
				DiscoveryDocumentRequest val2 = val;
				val3 = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync((HttpMessageInvoker)httpClient, val2, cancellationToken);
			}
			if (((ProtocolResponse)val3).IsError)
			{
				throw new AbpException($"Could not retrieve the OpenId Connect discovery document! ErrorType: {((ProtocolResponse)val3).ErrorType}. Error: {((ProtocolResponse)val3).Error}");
			}
			discoveryDocumentCacheItem = new IdentityModelDiscoveryDocumentCacheItem(val3.TokenEndpoint, val3.DeviceAuthorizationEndpoint);
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
		IdentityModelDiscoveryDocumentCacheItem val = await GetDiscoveryResponseAsync(config, cancellationToken);
		return new PasswordTokenRequest
		{
			Address = val.TokenEndpoint,
			Scope = config.Scope,
			ClientId = config.ClientId,
			ClientSecret = config.ClientSecret,
			UserName = config.UserName,
			Password = config.UserPassword
		};
	}

	protected virtual async Task<RefreshTokenRequest> CreateRefreshTokenRequestAsync(IdentityClientConfiguration config, string refreshToken, CancellationToken cancellationToken)
	{
		IdentityModelDiscoveryDocumentCacheItem val = await GetDiscoveryResponseAsync(config, cancellationToken);
		return new RefreshTokenRequest
		{
			Address = val.TokenEndpoint,
			Scope = config.Scope,
			ClientId = config.ClientId,
			ClientSecret = config.ClientSecret,
			RefreshToken = refreshToken
		};
	}

	protected virtual async Task<AuthorizationCodeTokenRequest> CreateAuthorizationCodeTokenRequestAsync(IdentityClientConfiguration config, string code, CancellationToken cancellationToken)
	{
		IdentityModelDiscoveryDocumentCacheItem val = await GetDiscoveryResponseAsync(config, cancellationToken);
		return new AuthorizationCodeTokenRequest
		{
			Address = val.TokenEndpoint,
			ClientId = config.ClientId,
			RedirectUri = ((Dictionary<string, string>)(object)config)["RedirectUri"],
			Code = code
		};
	}
}
