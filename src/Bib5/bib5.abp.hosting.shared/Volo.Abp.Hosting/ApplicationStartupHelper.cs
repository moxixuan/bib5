using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Volo.Abp.Hosting;

public static class ApplicationStartupHelper
{
	internal static Mutex? Mutex { get; private set; }

	public static bool IsRunning(string? appName = null)
	{
		Assembly assembly = Assembly.GetEntryAssembly() ?? new StackTrace().GetFrames().Last().GetMethod()?.Module.Assembly;
		Mutex = new Mutex(initiallyOwned: true, appName ?? assembly?.GetName().Name, out var createdNew);
		return !createdNew;
	}
}
