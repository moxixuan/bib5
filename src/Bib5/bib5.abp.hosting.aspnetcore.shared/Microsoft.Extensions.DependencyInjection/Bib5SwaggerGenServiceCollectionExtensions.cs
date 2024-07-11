using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5SwaggerGenServiceCollectionExtensions
{
	public static IServiceCollection AddBib5SwaggerGen(this IServiceCollection services)
	{
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(services);
		SwaggerGenServiceCollectionExtensions.AddSwaggerGen(services, (Action<SwaggerGenOptions>)delegate(SwaggerGenOptions options)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OpenApiInfo val = new OpenApiInfo();
			configuration.Bind("Swagger:OpenApiInfo", val);
			SwaggerGenOptionsExtensions.SwaggerDoc(options, configuration["Swagger:Name"], val);
			SwaggerGenOptionsExtensions.DocInclusionPredicate(options, (Func<string, ApiDescription, bool>)((string docName, ApiDescription description) => true));
			SwaggerGenOptionsExtensions.CustomSchemaIds(options, (Func<Type, string>)((Type type) => type.FullName));
		});
		return services;
	}

	public static IServiceCollection AddBib5SwaggerGenWithOAuth(this IServiceCollection services, Action<SwaggerGenOptions>? setupAction = null)
	{
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(services);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		configuration.Bind("AuthServer:SwaggerScopes", dictionary);
		if (setupAction == null)
		{
			AbpSwaggerGenServiceCollectionExtensions.AddAbpSwaggerGenWithOAuth(services, configuration["AuthServer:Authority"], dictionary, (Action<SwaggerGenOptions>)delegate(SwaggerGenOptions options)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Expected O, but got Unknown
				OpenApiInfo val = new OpenApiInfo();
				configuration.Bind("Swagger:OpenApiInfo", val);
				SwaggerGenOptionsExtensions.SwaggerDoc(options, configuration["Swagger:Name"], val);
				SwaggerGenOptionsExtensions.DocInclusionPredicate(options, (Func<string, ApiDescription, bool>)((string docName, ApiDescription description) => true));
				SwaggerGenOptionsExtensions.CustomSchemaIds(options, (Func<Type, string>)((Type type) => type.FullName));
			}, "/connect/authorize", "/connect/token");
		}
		else
		{
			AbpSwaggerGenServiceCollectionExtensions.AddAbpSwaggerGenWithOAuth(services, configuration["AuthServer:Authority"], dictionary, setupAction, "/connect/authorize", "/connect/token");
		}
		return services;
	}
}
