using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Windows;
using Bib5.Abp.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Volo.Abp.Modularity;

namespace Volo.Abp.Hosting;

[SupportedOSPlatform("windows")]
public class Bib5HostingWindowsModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		(this).Configure<RegistryEnvironmentOptions>(ServiceCollectionConfigurationExtensions.GetConfiguration(context.Services));
	}

	public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
	{
		IConfiguration requiredService = context.ServiceProvider.GetRequiredService<IConfiguration>();
		if (context.ServiceProvider.GetRequiredService<IOptions<RegistryEnvironmentOptions>>().Value.RegistryEnvironment.Enable)
		{
			string appName = Bib5AppConfigurationExtensions.GetAppName(requiredService);
			SetSecurity("Users", ApplicationEnvironmentHelper.GetLogRoot(appName));
			SetSecurity("Users", ApplicationEnvironmentHelper.GetDataRoot(appName));
			if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + appName) == null)
			{
				RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\" + appName);
				registryKey.SetValue("ConfigRoot", ApplicationEnvironmentHelper.GetConfigRoot(appName));
				registryKey.SetValue("DataRoot", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName ?? ""));
			}
		}
		static void SetSecurity(string identity, string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			DirectorySecurity accessControl = directoryInfo.GetAccessControl(AccessControlSections.All);
			InheritanceFlags inheritanceFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
			FileSystemAccessRule rule = new FileSystemAccessRule(identity, FileSystemRights.Modify, inheritanceFlags, PropagationFlags.None, AccessControlType.Allow);
			accessControl.ModifyAccessRule(AccessControlModification.Add, rule, out var _);
			directoryInfo.SetAccessControl(accessControl);
		}
	}
}
