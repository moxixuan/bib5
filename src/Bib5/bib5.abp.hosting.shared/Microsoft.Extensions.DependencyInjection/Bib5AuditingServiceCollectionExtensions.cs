using Microsoft.Extensions.Configuration;
using Volo.Abp.Auditing;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5AuditingServiceCollectionExtensions
{
	public static IServiceCollection AddBib5Auditing(this IServiceCollection services)
	{
		IServiceCollection services2 = services;
		services2.Configure(delegate(AbpAuditingOptions options)
		{
			options.ApplicationName = ServiceCollectionConfigurationExtensions.GetConfiguration(services2).GetAppName();
		});
		return services2;
	}
}
