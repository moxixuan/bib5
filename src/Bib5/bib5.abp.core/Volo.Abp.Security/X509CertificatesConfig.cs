using System.Collections.Generic;

namespace Volo.Abp.Security;

public class X509CertificatesConfig<TX509CertificateConfig>
{
	public Dictionary<string, TX509CertificateConfig> Certificates { get; set; } = new Dictionary<string, TX509CertificateConfig>();

}
