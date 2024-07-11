using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Bib5.Abp.Sessions;

[DependsOn(new Type[] { typeof(Bib5SessionClientDomainModule) })]
public class Bib5SessionClientDomainDesktopModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<SessionOptions>((Action<SessionOptions>)delegate
		{
		});
		(this).Configure<AbpAutoMapperOptions>((Action<AbpAutoMapperOptions>)delegate(AbpAutoMapperOptions options)
		{
			options.AddMaps<Bib5SessionClientDomainDesktopModule>(false);
		});
	}
}
