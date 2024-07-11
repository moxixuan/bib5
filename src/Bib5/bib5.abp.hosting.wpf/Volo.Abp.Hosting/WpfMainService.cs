using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Hosting;

public class WpfMainService : BasicMainService
{
	public WpfMainService(IAbpLazyServiceProvider lazyServiceProvider)
		: base(lazyServiceProvider)
	{
	}

	public override async Task<int> RunAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		_ = 2;
		try
		{
			IMainWindow mainWindow = (this).LazyServiceProvider.LazyGetService<IMainWindow>();
			SemaphoreSlim semaphoreMainWindow = null;
			if (mainWindow != null)
			{
				semaphoreMainWindow = new SemaphoreSlim(0);
				mainWindow.Show();
				mainWindow.Closed += delegate
				{
					semaphoreMainWindow.Release();
				};
			}
			NotifyIcon notifyIcon = (this).LazyServiceProvider.LazyGetService<NotifyIcon>();
			SemaphoreSlim semaphoreNotifyIcon = null;
			if (notifyIcon != null)
			{
				semaphoreNotifyIcon = new SemaphoreSlim(0);
				notifyIcon.Disposed += delegate
				{
					semaphoreNotifyIcon.Release();
				};
				if (semaphoreMainWindow != null)
				{
					await semaphoreMainWindow.WaitAsync(cancellationToken);
					notifyIcon.Dispose();
					semaphoreMainWindow.Release();
				}
			}
			if (semaphoreMainWindow != null)
			{
				await semaphoreMainWindow.WaitAsync(cancellationToken);
			}
			if (semaphoreNotifyIcon != null)
			{
				await semaphoreNotifyIcon.WaitAsync(cancellationToken);
			}
			return 0;
		}
		catch
		{
			return 1;
		}
	}
}
