using System;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5CorsServiceCollectionExtensions
{
	public static IServiceCollection AddBib5Cors(this IServiceCollection services)
	{
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(services);
		services.AddCors(delegate(CorsOptions options)
		{
			options.AddDefaultPolicy(delegate(CorsPolicyBuilder builder)
			{
				string text = configuration["App:CorsOrigins"] ?? "";
				AbpCorsPolicyBuilderExtensions.WithAbpExposedHeaders(builder.WithOrigins((from o in text.Split(",", StringSplitOptions.RemoveEmptyEntries)
					select AbpStringExtensions.RemovePostFix(o, new string[1] { "/" })).ToArray())).SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
			});
		});
		return services;
	}
}
