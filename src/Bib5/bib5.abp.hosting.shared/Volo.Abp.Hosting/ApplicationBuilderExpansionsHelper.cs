using System;
using System.IO;
using Bib5.Abp.Hosting.Contracts;
using Microsoft.Extensions.Configuration;

namespace Volo.Abp.Hosting;

public static class ApplicationBuilderExpansionsHelper
{
	public static void UseConfigRoot(IConfigurationBuilder configuration, string appName, string environmentName)
	{
		if (!ApplicationEnvironmentHelper.IsSetConfigRoot())
		{
			return;
		}
		string configRoot = ApplicationEnvironmentHelper.GetConfigRoot(appName);
		string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.json", SearchOption.TopDirectoryOnly);
		foreach (string text in files)
		{
			string text2 = Path.Combine(configRoot, text.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Trim(Path.DirectorySeparatorChar));
			if (!File.Exists(text2))
			{
				string directoryName = Path.GetDirectoryName(text2);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.Copy(text, text2);
			}
			else if (File.GetLastWriteTime(text) > File.GetLastWriteTime(text2))
			{
				File.Copy(text, text2, overwrite: true);
			}
		}
	}

	public static void UseDataRoot(string appName)
	{
		if (ApplicationEnvironmentHelper.IsSetDataRoot())
		{
			string dataRoot = ApplicationEnvironmentHelper.GetDataRoot(appName);
			if (!Directory.Exists(dataRoot))
			{
				Directory.CreateDirectory(dataRoot);
			}
			Directory.SetCurrentDirectory(dataRoot);
		}
	}
}
