using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Bib5.Abp.Sessions;

[DependsOn(new Type[] { typeof(AbpAccountHttpApiClientModule) })]
[DependsOn(new Type[] { typeof(Bib5SessionClientApplicationContractsModule) })]
public class Bib5SessionClientHttpApiClientModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		ServiceCollectionHttpClientProxyExtensions.AddHttpClientProxies(context.Services, typeof(Bib5SessionClientApplicationContractsModule).Assembly, "Bib5Session", true, (ApplicationServiceTypes)3);
		(this).Configure<AbpVirtualFileSystemOptions>((Action<AbpVirtualFileSystemOptions>)delegate(AbpVirtualFileSystemOptions options)
		{
			VirtualFileSetListExtensions.AddEmbedded<Bib5SessionClientHttpApiClientModule>(options.FileSets, (string)null, (string)null);
		});
	}
}
