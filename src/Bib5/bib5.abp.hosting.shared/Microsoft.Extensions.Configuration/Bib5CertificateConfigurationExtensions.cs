using System;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp;

namespace Microsoft.Extensions.Configuration;

public static class Bib5CertificateConfigurationExtensions
{
	public static bool HasCertificate(this IConfiguration configuration, string name)
	{
		return !AbpStringExtensions.IsNullOrWhiteSpace(configuration["Certificates:" + name + ":Certificate"]);
	}

	public static X509Certificate2 GetCertificate(this IConfiguration configuration)
	{
		return configuration.GetCertificate("Default");
	}

	public static X509Certificate2 GetCertificate(this IConfiguration configuration, string name)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		string? text = configuration["Certificates:" + name + ":Certificate"];
		if (AbpStringExtensions.IsNullOrWhiteSpace(text))
		{
			throw new AbpException("配置[\"Certificates:" + name + ":Certificat\"]中找不到证书" + name);
		}
		return new X509Certificate2(Convert.FromBase64String(text), configuration["Certificates:" + name + ":CertificatePassword"]);
	}

	public static X509Certificate2 GetCertificateOrDefault(this IConfiguration configuration, string name)
	{
		string text = configuration["Certificates:" + name + ":Certificate"];
		if (AbpStringExtensions.IsNullOrWhiteSpace(text))
		{
			return configuration.GetCertificate();
		}
		return new X509Certificate2(Convert.FromBase64String(text), configuration["Certificates:" + name + ":CertificatePassword"]);
	}

	public static X509Certificate2? FindCertificateOrDefault(this IConfiguration configuration, string name)
	{
		if (configuration.HasCertificate(name))
		{
			return configuration.GetCertificate(name);
		}
		if (configuration.HasCertificate("Default"))
		{
			return configuration.GetCertificate();
		}
		return null;
	}
}
