using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting.AspNetCore;

public static class ApplicationBuilderHelper
{
	public static async Task<WebApplication> BuildApplicationAsync<TStartupModule>(string[] args, Action<WebApplicationBuilder>? preAction = null, Action<WebApplicationBuilder>? postAction = null, Action<AbpApplicationCreationOptions>? optionsAction = null) where TStartupModule : IAbpModule
	{
		Action<AbpApplicationCreationOptions> optionsAction2 = optionsAction;
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		preAction?.Invoke(builder);
		string appName = builder.Configuration.GetAppName();
		string environmentName = builder.Environment.EnvironmentName;
		ApplicationBuilderExpansionsHelper.UseConfigRoot(builder.Configuration, appName, environmentName);
		ApplicationBuilderExpansionsHelper.UseDataRoot(appName);
		AbpAutofacHostBuilderExtensions.UseAutofac(AbpHostingHostBuilderExtensions.AddAppSettingsSecretsJson((IHostBuilder)builder.Host, true, true, "appsettings.secrets.json"));
		builder.Logging.ClearProviders().AddBib5Serilog();
		await WebApplicationBuilderExtensions.AddApplicationAsync<TStartupModule>(builder, (Action<AbpApplicationCreationOptions>)delegate(AbpApplicationCreationOptions options)
		{
			options.Environment = environmentName;
			options.ApplicationName = appName;
			optionsAction2?.Invoke(options);
		});
		builder.WebHost.ConfigureKestrel(delegate(WebHostBuilderContext context, KestrelServerOptions options)
		{
			WebHostBuilderContext context2 = context;
			if (context2.Configuration.HasCertificate("Https"))
			{
				options.ConfigureHttpsDefaults(delegate(HttpsConnectionAdapterOptions o)
				{
					o.ServerCertificate = context2.Configuration.GetCertificate("Https");
				});
			}
		});
		postAction?.Invoke(builder);
		return builder.Build();
	}
}
