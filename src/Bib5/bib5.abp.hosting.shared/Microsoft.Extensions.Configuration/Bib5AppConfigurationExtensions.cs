using System;

namespace Microsoft.Extensions.Configuration;

public static class Bib5AppConfigurationExtensions
{
	public static string GetAppName(this IConfiguration configuration)
	{
		string text = configuration["App:AppName"];
		if (AbpStringExtensions.IsNullOrWhiteSpace(text))
		{
			text = "Bib5App";
		}
		return text;
	}
}
