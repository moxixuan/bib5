using System;
using System.Security.Cryptography.X509Certificates;

namespace Volo.Abp.Security;

public class PfxCertificateConfig : IX509CertificateConfig
{
	public string Certificate { get; set; } = string.Empty;


	public string CertificatePassword { get; set; } = string.Empty;


	public X509Certificate2 ToX509Certificate2()
	{
		return new X509Certificate2(Convert.FromBase64String(Certificate), CertificatePassword);
	}
}
