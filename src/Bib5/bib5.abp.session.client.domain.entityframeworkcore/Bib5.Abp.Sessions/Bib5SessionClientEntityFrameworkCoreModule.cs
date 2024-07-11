using System;
using Bib5.Abp.Sessions.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Modularity;

namespace Bib5.Abp.Sessions;

[DependsOn(new Type[] { typeof(Bib5SessionClientDomainModule) })]
[DependsOn(new Type[] { typeof(AbpEntityFrameworkCoreModule) })]
public class Bib5SessionClientEntityFrameworkCoreModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		AbpEfCoreServiceCollectionExtensions.AddAbpDbContext<Bib5SessionDbContext>(context.Services, (Action<IAbpDbContextRegistrationOptionsBuilder>)delegate(IAbpDbContextRegistrationOptionsBuilder options)
		{
			((IAbpCommonDbContextRegistrationOptionsBuilder)options).AddDefaultRepositories(true);
		});
	}
}
