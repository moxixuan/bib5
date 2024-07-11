using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class Bib5SwaggerUIBuilderExtensions
{
	public static IApplicationBuilder UseBib5SwaggerUI(this IApplicationBuilder app, Action<SwaggerUIOptions>? setupAction = null)
	{
		IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
		if (setupAction == null)
		{
			AbpSwaggerUIBuilderExtensions.UseAbpSwaggerUI(app, (Action<SwaggerUIOptions>)delegate(SwaggerUIOptions options)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Expected O, but got Unknown
				UrlDescriptor val = new UrlDescriptor();
				configuration.Bind("Swagger:Endpoint", val);
				if (AbpStringExtensions.IsNullOrWhiteSpace(val.Name))
				{
					val.Name = Assembly.GetEntryAssembly()?.FullName;
				}
				if (AbpStringExtensions.IsNullOrWhiteSpace(val.Url))
				{
					val.Url = "/swagger/v1/swagger.json";
				}
				SwaggerUIOptionsExtensions.SwaggerEndpoint(options, val.Url, val.Name);
			});
		}
		else
		{
			AbpSwaggerUIBuilderExtensions.UseAbpSwaggerUI(app, setupAction);
		}
		return app;
	}

	public static IApplicationBuilder UseBib5SwaggerUIWithOAuth(this IApplicationBuilder app, Action<SwaggerUIOptions>? setupAction = null)
	{
		IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
		if (setupAction == null)
		{
			AbpSwaggerUIBuilderExtensions.UseAbpSwaggerUI(app, (Action<SwaggerUIOptions>)delegate(SwaggerUIOptions options)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Expected O, but got Unknown
				UrlDescriptor val = new UrlDescriptor();
				configuration.Bind("Swagger:Endpoint", val);
				if (AbpStringExtensions.IsNullOrWhiteSpace(val.Name))
				{
					val.Name = Assembly.GetEntryAssembly()?.FullName;
				}
				if (AbpStringExtensions.IsNullOrWhiteSpace(val.Url))
				{
					val.Url = "/swagger/v1/swagger.json";
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				configuration.Bind("AuthServer:SwaggerScopes", dictionary);
				SwaggerUIOptionsExtensions.SwaggerEndpoint(options, val.Url, val.Name);
				SwaggerUIOptionsExtensions.OAuthClientId(options, configuration["AuthServer:SwaggerClientId"]);
				SwaggerUIOptionsExtensions.OAuthClientSecret(options, configuration["AuthServer:SwaggerClientSecret"]);
				SwaggerUIOptionsExtensions.OAuthScopes(options, dictionary.Keys.ToArray());
			});
		}
		else
		{
			AbpSwaggerUIBuilderExtensions.UseAbpSwaggerUI(app, setupAction);
		}
		return app;
	}
}
