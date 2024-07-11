using Volo.Abp.BackgroundJobs;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5BackgroundJobServiceCollectionExtensions
{
	public static IServiceCollection ConfigureBib5BackgroundJob(this IServiceCollection services)
	{
		services.Configure(delegate(AbpBackgroundJobOptions options)
		{
			options.IsJobExecutionEnabled = true;
		});
		return services;
	}
}
