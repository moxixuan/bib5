using System;
using Bib5.Abp.Hosting.Console.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Bib5.Abp.Hosting.Console;

[DependsOn(new Type[] { typeof(Bib5HostingConsoleDomainModule) })]
[DependsOn(new Type[] { typeof(AbpSettingManagementEntityFrameworkCoreModule) })]
public class Bib5AbpHostingConsoleEntityFrameworkCoreModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		AbpEfCoreServiceCollectionExtensions.AddAbpDbContext<Bib5AbpHostingConsoleDbContext>(context.Services, (Action<IAbpDbContextRegistrationOptionsBuilder>)delegate(IAbpDbContextRegistrationOptionsBuilder options)
		{
			((IAbpCommonDbContextRegistrationOptionsBuilder)options).AddDefaultRepositories(true);
		});
	}
}
