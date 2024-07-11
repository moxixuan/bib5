using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Bib5.Abp.Cli;

public static class CliEnvironmentExtensions
{
	public static void AddCliEnvironment(this AbpApplicationCreationOptions options, CliEnvironment? environment = null)
	{
		options.Services.AddCliEnvironment(environment ?? new CliEnvironment());
	}

	public static void AddCliEnvironment(this IServiceCollection services, CliEnvironment? environment = null)
	{
		ServiceCollectionObjectAccessorExtensions.AddObjectAccessor<CliEnvironment>(services, environment ?? new CliEnvironment());
	}

	public static CliEnvironment? GetCliEnvironment(this IServiceCollection services)
	{
		return ServiceCollectionObjectAccessorExtensions.GetObjectOrNull<CliEnvironment>(services);
	}

	public static bool IsCliEnvironment(this IServiceCollection services)
	{
		return services.GetCliEnvironment() != null;
	}

	public static CliEnvironment? GetCliEnvironment(this IServiceProvider serviceProvider)
	{
		return serviceProvider.GetService<IObjectAccessor<CliEnvironment>>()?.Value;
	}

	public static bool IsCliEnvironment(this IServiceProvider serviceProvider)
	{
		return serviceProvider.GetCliEnvironment() != null;
	}
}
