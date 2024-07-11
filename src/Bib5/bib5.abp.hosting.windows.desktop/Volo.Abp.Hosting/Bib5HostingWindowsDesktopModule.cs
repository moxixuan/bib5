using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(Bib5HostingDotNetSimplifiedModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingWindowsModule) })]
public class Bib5HostingWindowsDesktopModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<StartupOptions>(ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services));
	}
}
