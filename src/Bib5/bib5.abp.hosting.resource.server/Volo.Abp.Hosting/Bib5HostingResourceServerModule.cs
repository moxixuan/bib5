using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

public class Bib5HostingResourceServerModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddBib5Authentication();
	}

	public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
	{
		ApplicationInitializationContextExtensions.GetApplicationBuilder(context).UseAuthentication();
	}

	public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
	{
		ApplicationInitializationContextExtensions.GetApplicationBuilder(context).UseAuthorization();
	}
}
