using System.IO.StreamTransfers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Security;

namespace Volo.Abp;

public class Bib5AbpCoreDomainModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services);
		(this).Configure<X509CertificatesConfig<PfxCertificateConfig>>(configuration);
		(this).Configure<StreamTransferConfig>((IConfiguration)configuration.GetSection("StreamTransfer"));
	}
}
