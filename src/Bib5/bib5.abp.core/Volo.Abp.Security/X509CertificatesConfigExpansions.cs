using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Volo.Abp.Security;

public static class X509CertificatesConfigExpansions
{
	public static X509Certificate2 GetOrDefault<TX509CertificateConfig>(this X509CertificatesConfig<TX509CertificateConfig> config, string name) where TX509CertificateConfig : IX509CertificateConfig
	{
		TX509CertificateConfig orDefault = AbpDictionaryExtensions.GetOrDefault<string, TX509CertificateConfig>(config.Certificates, name);
		return ((orDefault != null) ? orDefault : AbpDictionaryExtensions.GetOrDefault<string, TX509CertificateConfig>(config.Certificates, "Default")).ToX509Certificate2();
	}
}
