using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bib5.Abp.Hosting.Contracts;

public static class ApplicationEnvironmentHelper
{
	private static Lazy<IConfiguration> _configuration;

	static ApplicationEnvironmentHelper()
	{
		_configuration = new Lazy<IConfiguration>(delegate
		{
			ConfigurationManager configurationManager = new ConfigurationManager();
			configurationManager.AddJsonFile("appsettings.json");
			configurationManager.AddEnvironmentVariables("ASPNETCORE_");
			string text = configurationManager[HostDefaults.EnvironmentKey];
			if (AbpStringExtensions.IsNullOrEmpty(text))
			{
				configurationManager.AddEnvironmentVariables("DOTNET_");
				text = configurationManager[HostDefaults.EnvironmentKey];
			}
			if (AbpStringExtensions.IsNullOrEmpty(text))
			{
				text = Environments.Production;
			}
			if (File.Exists("appsettings." + text + ".json"))
			{
				configurationManager.AddJsonFile("appsettings." + text + ".json");
			}
			return ((IConfigurationBuilder)configurationManager).Build();
		});
	}

	private static string? ExtractFromConfiguration(string key)
	{
		return _configuration.Value.GetValue<string>(key);
	}

	private static string? ExtractFromCommandLine(string argKey)
	{
		string argKey2 = argKey;
		string result = null;
		int num = AbpListExtensions.FindIndex<string>((IList<string>)Environment.GetCommandLineArgs(), (Predicate<string>)((string arg) => arg.Equals(argKey2)));
		if (num >= 0 && Environment.GetCommandLineArgs().Length > num)
		{
			result = Environment.GetCommandLineArgs()[num + 1];
		}
		return result;
	}

	private static string? GetValue(string key)
	{
		string text = ExtractFromCommandLine("--" + key.ToLowerInvariant());
		if (text == null)
		{
			text = Environment.GetEnvironmentVariable(key);
		}
		if (text == null)
		{
			text = ExtractFromConfiguration(key);
		}
		return text;
	}

	public static string GetConfigRoot(string appName)
	{
		string text = GetValue("CONFIG_ROOT");
		if (AbpStringExtensions.IsNullOrWhiteSpace(text))
		{
			return Directory.GetCurrentDirectory();
		}
		if ("default".Equals(text))
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				text = Path.Combine(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)), "." + appName.ToLowerInvariant());
			}
			else
			{
				if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					throw new Exception("当前系统暂不支持通过环境变量默认值(CONFIG_ROOT=default})设置配置根目录");
				}
				text = Path.Combine("/etc/" + appName.ToLowerInvariant());
			}
		}
		return text;
	}

	public static bool IsSetConfigRoot()
	{
		return !AbpStringExtensions.IsNullOrWhiteSpace(GetValue("CONFIG_ROOT"));
	}

	public static string GetLogRoot(string appName)
	{
		string text = GetValue("LOG_ROOT");
		if (AbpStringExtensions.IsNullOrWhiteSpace(text))
		{
			return Directory.GetCurrentDirectory();
		}
		if ("default".Equals(text))
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				string text2 = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
				text = ((!AbpStringExtensions.IsNullOrEmpty(text2)) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), text2, appName, "logs") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName, "logs"));
			}
			else
			{
				if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					throw new Exception("当前系统暂不支持通过环境变量默认值(LOG_ROOT=default})设置日志根目录");
				}
				text = Path.Combine("/var/log/" + appName.ToLowerInvariant());
			}
		}
		return text;
	}

	public static bool IsSetLogRoot()
	{
		return !AbpStringExtensions.IsNullOrWhiteSpace(GetValue("LOG_ROOT"));
	}

	public static string GetDataRoot(string appName)
	{
		string text = GetValue("DATA_ROOT");
		if (AbpStringExtensions.IsNullOrWhiteSpace(text))
		{
			return Directory.GetCurrentDirectory();
		}
		if ("default".Equals(text))
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				string text2 = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
				text = ((!AbpStringExtensions.IsNullOrEmpty(text2)) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), text2, appName, "datas") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName, "datas"));
			}
			else
			{
				if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					throw new Exception("当前系统暂不支持通过环境变量默认值(DATA_ROOT=default})设置数据路径根目录");
				}
				text = Path.Combine("/var/lib/" + appName.ToLowerInvariant());
			}
		}
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static bool IsSetDataRoot()
	{
		return !AbpStringExtensions.IsNullOrWhiteSpace(GetValue("DATA_ROOT"));
	}
}
