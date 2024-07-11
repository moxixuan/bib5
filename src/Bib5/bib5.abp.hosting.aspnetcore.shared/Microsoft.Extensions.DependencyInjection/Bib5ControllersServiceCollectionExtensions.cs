using System;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5ControllersServiceCollectionExtensions
{
	public static IServiceCollection ConfigureBib5ConventionalControllers<TApplicationModule>(this IServiceCollection services) where TApplicationModule : AbpModule
	{
		services.Configure(delegate(AbpAspNetCoreMvcOptions options)
		{
			options.ConventionalControllers.Create(typeof(TApplicationModule).Assembly, (Action<ConventionalControllerSetting>)null);
		});
		return services;
	}
}
