using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(AbpAutofacModule) })]
[DependsOn(new Type[] { typeof(AbpDataModule) })]
[DependsOn(new Type[] { typeof(AbpMultiTenancyModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingSharedModule) })]
public class Bib5HostingDotNetSharedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		ServiceCollectionObjectAccessorExtensions.AddObjectAccessor<IHost>(context.Services);
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		IHostApplicationLifetime hostApplicationLifetime = context.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
		ILogger<Bib5HostingSharedModule> logger = context.ServiceProvider.GetRequiredService<ILogger<Bib5HostingSharedModule>>();
		IMainService mainService = context.ServiceProvider.GetRequiredService<IMainService>();
		hostApplicationLifetime.ApplicationStarted.Register(async delegate
		{
			try
			{
				await mainService.RunAsync();
				hostApplicationLifetime.StopApplication();
			}
			catch (Exception ex)
			{
				AbpLoggerExtensions.LogException((ILogger)logger, ex, (LogLevel?)null);
			}
		}, useSynchronizationContext: false);
		await Task.CompletedTask;
	}
}
