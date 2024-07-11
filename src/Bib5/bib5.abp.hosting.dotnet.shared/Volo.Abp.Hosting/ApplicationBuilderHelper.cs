using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

public static class ApplicationBuilderHelper
{
	public static async Task<IHost> BuildApplicationAsync<TStartupModule>(string[] args, Action<HostApplicationBuilder>? builderAction = null, Action<AbpApplicationCreationOptions>? optionsAction = null) where TStartupModule : IAbpModule
	{
		Action<AbpApplicationCreationOptions> optionsAction2 = optionsAction;
		Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
		HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
		string appName = builder.Configuration.GetAppName();
		string environmentName = builder.Environment.EnvironmentName;
		ApplicationBuilderExpansionsHelper.UseConfigRoot(builder.Configuration, appName, environmentName);
		ApplicationBuilderExpansionsHelper.UseDataRoot(appName);
		builder.Logging.ClearProviders().AddBib5Serilog();
		ContainerBuilder val = new ContainerBuilder();
		ServiceCollectionObjectAccessorExtensions.AddObjectAccessor<ContainerBuilder>(builder.Services, val);
		builder.ConfigureContainer((IServiceProviderFactory<ContainerBuilder>)new AbpAutofacServiceProviderFactory(val));
		await ServiceCollectionApplicationExtensions.AddApplicationAsync<TStartupModule>(builder.Services, (Action<AbpApplicationCreationOptions>)delegate(AbpApplicationCreationOptions options)
		{
			ServiceCollectionConfigurationExtensions.ReplaceConfiguration(options.Services, (IConfiguration)builder.Configuration);
			options.Environment = environmentName;
			options.ApplicationName = appName;
			optionsAction2?.Invoke(options);
		});
		builderAction?.Invoke(builder);
		return builder.Build();
	}
}
