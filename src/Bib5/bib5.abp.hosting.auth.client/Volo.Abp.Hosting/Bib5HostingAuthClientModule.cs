using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

public class Bib5HostingAuthClientModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		(this).PreConfigure<AbpHttpClientBuilderOptions>((Action<AbpHttpClientBuilderOptions>)delegate(AbpHttpClientBuilderOptions options)
		{
			options.ProxyClientBuildActions.Add(delegate(string _, IHttpClientBuilder builder)
			{
				builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
				{
					ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
				});
			});
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services);
		context.Services.AddHttpClient(configuration["AuthServer:Authority"]).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		});
	}
}
