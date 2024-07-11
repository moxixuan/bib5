using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Volo.Abp.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5JwtBearerServiceCollectionExtensions
{
	public static IServiceCollection AddBib5Authentication(this IServiceCollection services, string? audience = null)
	{
		string audience2 = audience;
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(services);
		JwtBearerExtensions.AddJwtBearer(services.AddAuthentication(delegate(AuthenticationOptions options)
		{
			options.DefaultScheme = "Bearer";
		}), (Action<JwtBearerOptions>)delegate(JwtBearerOptions options)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			options.TokenValidationParameters.ValidateAudience = false;
			options.BackchannelHttpHandler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};
			options.Authority = configuration["AuthServer:Authority"];
			options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
			if (AbpStringExtensions.IsNullOrWhiteSpace(audience2))
			{
				options.Audience = configuration["AuthServer:Audience"];
			}
			else
			{
				options.Audience = audience2;
			}
			if (AbpStringExtensions.IsNullOrWhiteSpace(options.Audience))
			{
				throw new Exception("配置项[\"AuthServer:Audience\"]不能为空");
			}
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = delegate(MessageReceivedContext context)
				{
					StringValues stringValues = ((BaseContext<JwtBearerOptions>)(object)context).Request.Query["access_token"];
					if (!string.IsNullOrEmpty(stringValues) && (((BaseContext<JwtBearerOptions>)(object)context).HttpContext.WebSockets.IsWebSocketRequest || ((BaseContext<JwtBearerOptions>)(object)context).Request.Headers.Accept == "text/event-stream"))
					{
						ILogger<JwtBearerEvents> requiredService = ((BaseContext<JwtBearerOptions>)(object)context).HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
						object[] args = (string?[]?)stringValues;
						requiredService.LogDebug("JwtBearerEvents access_token {0}", args);
						PathString path = ((BaseContext<JwtBearerOptions>)(object)context).HttpContext.Request.Path;
						string text = configuration["SignalR:Prefix"];
						if (text == null)
						{
							text = "/hub";
						}
						if (path.StartsWithSegments(text))
						{
							context.Token = stringValues;
							requiredService.LogDebug("JwtBearerEvents set access_token");
						}
					}
					return Task.CompletedTask;
				}
			};
		});
		services.Configure(delegate(AbpClaimsPrincipalFactoryOptions options)
		{
			options.IsDynamicClaimsEnabled = true;
		});
		return services;
	}
}
