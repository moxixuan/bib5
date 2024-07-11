using System;
using Bib5.Abp.Session.Localization;
using Bib5.Abp.Sessions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Bib5.Abp.Session;

[DependsOn(new Type[] { typeof(Bib5AbpCoreDomainModule) })]
public class Bib5SessionClientDomainSharedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<AbpVirtualFileSystemOptions>((Action<AbpVirtualFileSystemOptions>)delegate(AbpVirtualFileSystemOptions options)
		{
			VirtualFileSetListExtensions.AddEmbedded<Bib5SessionClientDomainSharedModule>(options.FileSets, (string)null, (string)null);
		});
		(this).Configure<AbpLocalizationOptions>((Action<AbpLocalizationOptions>)delegate(AbpLocalizationOptions options)
		{
			LocalizationResourceExtensions.AddVirtualJson<LocalizationResource>(options.Resources.Add<Bib5SessionResource>("en"), "/Bib5/Abp/Sessions/Localization/HttpClient");
			options.DefaultResourceType = typeof(Bib5SessionResource);
		});
		(this).Configure<AbpExceptionLocalizationOptions>((Action<AbpExceptionLocalizationOptions>)delegate(AbpExceptionLocalizationOptions options)
		{
			options.MapCodeNamespace("Bib5HttpClient", typeof(Bib5SessionResource));
		});
		(this).Configure<SessionOptions>(ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services));
	}
}
