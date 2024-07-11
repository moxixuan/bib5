using System;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Volo.Abp.Hosting.AspNetCore;

[Dependency(ReplaceServices = true)]
public class Bib5BrandingProvider : DefaultBrandingProvider
{
	private const string DefaultAppName = "Bib5App";

	public override string AppName { get; }

	public Bib5BrandingProvider(IConfiguration configuration)
	{
		AppName = configuration["App:AppName"];
		if (AbpStringExtensions.IsNullOrWhiteSpace(((DefaultBrandingProvider)this).AppName))
		{
			AppName = "Bib5App";
		}
	}
}
