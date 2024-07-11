using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Volo.Abp.Hosting.AspNetCore;

[DependsOn(new Type[] { typeof(AbpSwashbuckleModule) })]
[DependsOn(new Type[] { typeof(Bib5HostingSharedModule) })]
public class Bib5HostingAspNetCoreSharedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddBib5Cors().AddHealthChecks();
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		IApplicationBuilder applicationBuilder = ApplicationInitializationContextExtensions.GetApplicationBuilder(context);
		AbpApplicationBuilderExtensions.UseAbpRequestLocalization(applicationBuilder, (Action<RequestLocalizationOptions>)null);
		applicationBuilder.UseStaticFiles();
		applicationBuilder.UseRouting();
		applicationBuilder.UseCors();
		SwaggerBuilderExtensions.UseSwagger(applicationBuilder, (Action<SwaggerOptions>)null);
		AbpApplicationBuilderExtensions.UseUnitOfWork(applicationBuilder);
		applicationBuilder.UseHealthChecks("/health");
		AbpAspNetCoreSerilogApplicationBuilderExtensions.UseAbpSerilogEnrichers(applicationBuilder);
	}

	public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
	{
		AbpAspNetCoreApplicationBuilderExtensions.UseConfiguredEndpoints(ApplicationInitializationContextExtensions.GetApplicationBuilder(context), (Action<IEndpointRouteBuilder>)null);
	}
}
