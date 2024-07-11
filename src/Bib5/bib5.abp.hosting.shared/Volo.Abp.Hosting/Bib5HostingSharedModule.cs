using System;
using Bib5.Abp.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(AbpAspNetCoreSerilogModule) })]
[DependsOn(new Type[] { typeof(AbpAutofacModule) })]
[DependsOn(new Type[] { typeof(AbpDataModule) })]
[DependsOn(new Type[] { typeof(AbpUnitOfWorkModule) })]
public class Bib5HostingSharedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		IdentityModelEventSource.ShowPII = true;
		context.Services.ConfigureBib5BackgroundJob().ConfigureBib5Localization().AddBib5Auditing();
	}

	public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
	{
		IConfiguration configuration = ApplicationInitializationContextExtensions.GetConfiguration(context);
		string appName = configuration.GetAppName();
		ILogger<Bib5HostingSharedModule> logger = context.ServiceProvider.GetRequiredService<ILogger<Bib5HostingSharedModule>>();
		context.ServiceProvider.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(delegate
		{
			logger.LogInformation("Config root path: {0}", ApplicationEnvironmentHelper.GetConfigRoot(appName));
			logger.LogInformation("Data root path: {0}", ApplicationEnvironmentHelper.GetDataRoot(appName));
			logger.LogInformation("Log root path: {0}", ApplicationEnvironmentHelper.GetLogRoot(appName));
		});
	}
}
