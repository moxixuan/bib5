using System;
using System.Collections.Concurrent;
using Bib5.Abp.Session;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.IdentityModel;
using Volo.Abp.Modularity;

namespace Bib5.Abp.Sessions;

[DependsOn(new Type[] { typeof(AbpAutoMapperModule) })]
[DependsOn(new Type[] { typeof(AbpIdentityModelModule) })]
[DependsOn(new Type[] { typeof(Bib5SessionClientDomainSharedModule) })]
public class Bib5SessionClientDomainModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<SessionOptions>((Action<SessionOptions>)delegate
		{
		});
		(this).Configure<AbpAutoMapperOptions>((Action<AbpAutoMapperOptions>)delegate(AbpAutoMapperOptions options)
		{
			options.AddMaps<Bib5SessionClientDomainModule>(false);
		});
		ServiceCollectionObjectAccessorExtensions.AddObjectAccessor<ConcurrentDictionary<string, ChangeTokenInfo>>(context.Services, new ConcurrentDictionary<string, ChangeTokenInfo>());
	}
}
