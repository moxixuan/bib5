using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Volo.Abp.Domain.Services;

namespace System.Windows;

[SupportedOSPlatform("windows")]
public class StartupManager : DomainService
{
	private const string RegistryPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

	private bool _startup;

	protected IConfiguration Configuration { get; }

	protected IOptions<StartupOptions> Options { get; }

	public string AppName { get; }

	public bool IsAvailable { get; }

	public bool Startup
	{
		get
		{
			return _startup;
		}
		set
		{
			if (_startup == value)
			{
				return;
			}
			if (IsAvailable)
			{
				if (TaskService.Instance.Connected)
				{
					if (value)
					{
						CreateTask();
					}
					else
					{
						DisableTask();
						DeleteTask();
					}
					_startup = value;
					return;
				}
				try
				{
					if (value)
					{
						CreateRegistryKey();
					}
					else
					{
						DeleteRegistryKey();
					}
					_startup = value;
					return;
				}
				catch (UnauthorizedAccessException)
				{
					throw new InvalidOperationException();
				}
			}
			throw new InvalidOperationException();
		}
	}

	public StartupManager(IConfiguration configuration, IOptions<StartupOptions> options)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Configuration = configuration;
		Options = options;
		AppName = Configuration.GetAppName();
		if (!Options.Value.Startup.Enable)
		{
			IsAvailable = false;
			return;
		}
		if (Environment.OSVersion.Platform >= PlatformID.Unix)
		{
			IsAvailable = false;
			return;
		}
		if (IsAdministrator() && TaskService.Instance.Connected)
		{
			IsAvailable = true;
			Task task = GetTask();
			if (task == null)
			{
				return;
			}
			{
				foreach (Microsoft.Win32.TaskScheduler.Action action in task.Definition.Actions)
				{
					if ((int)action.ActionType == 0)
					{
						ExecAction val = (ExecAction)(object)((action is ExecAction) ? action : null);
						if (val != null && val.Path.Equals(Environment.ProcessPath, StringComparison.OrdinalIgnoreCase))
						{
							_startup = task.Enabled;
						}
					}
				}
				return;
			}
		}
		try
		{
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
			{
				string text = (string)registryKey?.GetValue(AppName);
				if (text != null)
				{
					_startup = text == Environment.ProcessPath;
				}
			}
			IsAvailable = true;
		}
		catch (SecurityException)
		{
			IsAvailable = false;
		}
	}

	private static bool IsAdministrator()
	{
		try
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}
		catch
		{
			return false;
		}
	}

	private Task? GetTask()
	{
		try
		{
			return TaskService.Instance.AllTasks.FirstOrDefault((Func<Task, bool>)((Task x) => x.Name.Equals(AppName, StringComparison.OrdinalIgnoreCase)));
		}
		catch
		{
			return null;
		}
	}

	private void CreateTask()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		TaskDefinition val = TaskService.Instance.NewTask();
		if (AbpStringExtensions.IsNullOrWhiteSpace(Options.Value.Startup.Description))
		{
			val.RegistrationInfo.Description = "Starts " + AppName + " on Windows startup.";
		}
		else
		{
			val.RegistrationInfo.Description = Options.Value.Startup.Description;
		}
		val.Triggers.Add<LogonTrigger>(new LogonTrigger());
		val.Settings.StartWhenAvailable = true;
		val.Settings.DisallowStartIfOnBatteries = false;
		val.Settings.StopIfGoingOnBatteries = false;
		val.Settings.ExecutionTimeLimit = TimeSpan.Zero;
		val.Settings.AllowHardTerminate = false;
		val.Principal.RunLevel = (TaskRunLevel)1;
		val.Principal.LogonType = (TaskLogonType)3;
		val.Actions.Add<ExecAction>(new ExecAction(Environment.ProcessPath, Options.Value.Startup.Arguments, Path.GetDirectoryName(Environment.ProcessPath)));
		TaskService.Instance.RootFolder.RegisterTaskDefinition(AppName, val);
	}

	private void DisableTask()
	{
		Task task = GetTask();
		if (task != null)
		{
			task.Enabled = false;
		}
	}

	private void DeleteTask()
	{
		Task task = GetTask();
		if (task != null)
		{
			TaskFolder folder = task.Folder;
			if (folder != null)
			{
				folder.DeleteTask((task != null) ? task.Name : null, false);
			}
		}
	}

	private void CreateRegistryKey()
	{
		Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run")?.SetValue(AppName, Environment.ProcessPath);
	}

	private void DeleteRegistryKey()
	{
		Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run")?.DeleteValue(AppName);
	}
}
