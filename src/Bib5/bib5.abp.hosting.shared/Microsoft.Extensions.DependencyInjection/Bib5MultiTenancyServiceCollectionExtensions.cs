using Volo.Abp.MultiTenancy;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5MultiTenancyServiceCollectionExtensions
{
	public static IServiceCollection ConfigureBib5MultiTenancy(this IServiceCollection services)
	{
		services.Configure(delegate(AbpMultiTenancyOptions options)
		{
			options.IsEnabled = true;
		});
		return services;
	}
}
