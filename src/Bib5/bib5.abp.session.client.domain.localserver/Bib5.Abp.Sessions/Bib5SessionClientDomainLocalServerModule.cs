using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Modularity;
using Volo.Abp.Security;

namespace Bib5.Abp.Sessions;

[DependsOn(new Type[] { typeof(AbpSecurityModule) })]
[DependsOn(new Type[] { typeof(AbpHttpClientIdentityModelWebModule) })]
[DependsOn(new Type[] { typeof(Bib5SessionClientDomainModule) })]
public class Bib5SessionClientDomainLocalServerModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<SessionOptions>((Action<SessionOptions>)delegate
		{
		});
		(this).Configure<AbpAutoMapperOptions>((Action<AbpAutoMapperOptions>)delegate(AbpAutoMapperOptions options)
		{
			options.AddMaps<Bib5SessionClientDomainLocalServerModule>(false);
		});
	}
}
