using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Bib5.Abp.Console;

public static class ConsoleEnvironmentExtensions
{
	public static void AddConsoleEnvironment(this AbpApplicationCreationOptions options, ConsoleEnvironment? environment = null)
	{
		options.Services.AddConsoleEnvironment(environment ?? new ConsoleEnvironment());
	}

	public static void AddConsoleEnvironment(this IServiceCollection services, ConsoleEnvironment? environment = null)
	{
		ServiceCollectionObjectAccessorExtensions.AddObjectAccessor<ConsoleEnvironment>(services, environment ?? new ConsoleEnvironment());
	}

	public static ConsoleEnvironment? GetConsoleEnvironment(this IServiceCollection services)
	{
		return ServiceCollectionObjectAccessorExtensions.GetObjectOrNull<ConsoleEnvironment>(services);
	}

	public static bool IsConsoleEnvironment(this IServiceCollection services)
	{
		return services.GetConsoleEnvironment() != null;
	}

	public static ConsoleEnvironment? GetConsoleEnvironment(this IServiceProvider serviceProvider)
	{
		return serviceProvider.GetService<IObjectAccessor<ConsoleEnvironment>>()?.Value;
	}

	public static bool IsConsoleEnvironment(this IServiceProvider serviceProvider)
	{
		return serviceProvider.GetConsoleEnvironment() != null;
	}
}
