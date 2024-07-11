using System.IO;
using System.Security.Cryptography.X509Certificates;
using Bib5.Abp.Hosting.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5DataProtectionServiceCollectionExtensions
{
	public static IServiceCollection AddBib5SimplifiedDataProtection(this IServiceCollection services)
	{
		string appName = ServiceCollectionConfigurationExtensions.GetConfiguration(services).GetAppName();
		string configRoot = ApplicationEnvironmentHelper.GetConfigRoot(appName);
		IConfiguration configuration = ServiceCollectionConfigurationExtensions.GetConfiguration(services);
		IDataProtectionBuilder builder = services.AddDataProtection().SetApplicationName(appName).PersistKeysToFileSystem(new DirectoryInfo(configRoot));
		X509Certificate2 x509Certificate = configuration.FindCertificateOrDefault("DataProtection");
		if (x509Certificate != null)
		{
			builder.ProtectKeysWithCertificate(x509Certificate);
		}
		return services;
	}
}
