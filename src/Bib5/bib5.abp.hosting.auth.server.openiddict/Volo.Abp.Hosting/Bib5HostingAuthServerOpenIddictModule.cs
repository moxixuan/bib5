using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Volo.Abp.Hosting;

[DependsOn(new Type[] { typeof(Bib5HostingAuthServerModule) })]
public class Bib5HostingAuthServerOpenIddictModule : AbpModule
{
	private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services);
		(this).PreConfigure<OpenIddictServerBuilder>((Action<OpenIddictServerBuilder>)delegate(OpenIddictServerBuilder builder)
		{
			builder.SetIssuer(new Uri(configuration["OpenIddict:Issuer"]));
		});
		(this).PreConfigure<OpenIddictBuilder>((Action<OpenIddictBuilder>)delegate(OpenIddictBuilder builder)
		{
			OpenIddictBuilder builder2 = builder;
			OneTimeRunner.Run((Action)delegate
			{
				OpenIddictValidationExtensions.AddValidation(builder2, (Action<OpenIddictValidationBuilder>)delegate(OpenIddictValidationBuilder options)
				{
					options.AddAudiences(new string[1] { configuration["AuthServer:Audience"] });
					OpenIddictValidationServerIntegrationExtensions.UseLocalServer(options);
                    OpenIddictValidationAspNetCoreExtensions.UseAspNetCore(options);            
                });
			});
		});
	}
	    
}
