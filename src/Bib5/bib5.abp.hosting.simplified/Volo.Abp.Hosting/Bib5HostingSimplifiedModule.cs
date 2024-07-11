using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(Bib5HostingSharedModule) })]
public class Bib5HostingSimplifiedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddBib5SimplifiedDataProtection();
	}
}
