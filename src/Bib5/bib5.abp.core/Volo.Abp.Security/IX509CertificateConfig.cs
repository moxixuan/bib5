using System.Security.Cryptography.X509Certificates;

namespace Volo.Abp.Security;

public interface IX509CertificateConfig
{
	X509Certificate2 ToX509Certificate2();
}
