using System;
using Bib5.Abp.Hosting.Console.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Bib5.Abp.Hosting.Console;

public class Bib5HostingConsoleSharedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<AbpVirtualFileSystemOptions>((Action<AbpVirtualFileSystemOptions>)delegate(AbpVirtualFileSystemOptions options)
		{
			VirtualFileSetListExtensions.AddEmbedded<Bib5HostingConsoleSharedModule>(options.FileSets, (string)null, (string)null);
		});
		(this).Configure<AbpLocalizationOptions>((Action<AbpLocalizationOptions>)delegate(AbpLocalizationOptions options)
		{
			LocalizationResourceExtensions.AddVirtualJson<LocalizationResource>(options.Resources.Add<Bib5HostingConsoleResource>("en"), "/Bib5/Abp/Hosting/Console/Localization/Console");
			options.DefaultResourceType = typeof(Bib5HostingConsoleResource);
		});
		(this).Configure<AbpExceptionLocalizationOptions>((Action<AbpExceptionLocalizationOptions>)delegate(AbpExceptionLocalizationOptions options)
		{
			options.MapCodeNamespace("Bib5HostingConsole", typeof(Bib5HostingConsoleResource));
		});
	}
}
