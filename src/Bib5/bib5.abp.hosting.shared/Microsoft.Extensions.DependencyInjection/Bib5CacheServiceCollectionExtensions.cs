using System;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Caching;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5CacheServiceCollectionExtensions
{
	[Obsolete("无需要调用", true)]
	public static IServiceCollection ConfigureBib5DistributedCache(this IServiceCollection services)
	{
		IServiceCollection services2 = services;
		services2.Configure(delegate(AbpDistributedCacheOptions options)
		{
			options.KeyPrefix = ServiceCollectionConfigurationExtensions.GetConfiguration(services2).GetAppName() + ":";
		});
		return services2;
	}
}
