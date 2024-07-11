using System;
using System.Threading.Tasks;
using Bib5.Abp.Data;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DataSeeds.Options;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(AbpOpenIddictDomainModule) })]
[DependsOn(new Type[] { typeof(AbpPermissionManagementDomainModule) })]
[DependsOn(new Type[] { typeof(Bib5AbpCoreDomainModule) })]
public class Bib5HostingDataSeedOpenIddictModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<OpenIddictOptions>(ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services));
	}

	public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		await context.ServiceProvider.GetRequiredService<DefaultDbMigrationService>().MigrateAsync();
	}
}
