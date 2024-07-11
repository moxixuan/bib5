using System;
using Bib5.Abp.Hosting.Console.Sessions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Bib5.Abp.Hosting.Console;

[DependsOn(new Type[] { typeof(Bib5HostingConsoleSharedModule) })]
public class Bib5HostingConsoleDomainModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<SessionOptions>((Action<SessionOptions>)delegate
		{
		});
		(this).Configure<AbpAutoMapperOptions>((Action<AbpAutoMapperOptions>)delegate(AbpAutoMapperOptions options)
		{
			options.AddMaps<Bib5HostingConsoleDomainModule>(false);
		});
	}
}
