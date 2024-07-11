using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting.AspNetCore;

[DependsOn(new Type[] { typeof(Bib5HostingSimplifiedModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingAspNetCoreSharedModule) })]
public class Bib5HostingAspNetCoreSimplifiedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddBib5SwaggerGen();
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		ApplicationInitializationContextExtensions.GetApplicationBuilder(context).UseBib5SwaggerUI();
	}
}
