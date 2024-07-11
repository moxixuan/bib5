using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Hosting;

[Obsolete("不使用后面服务方式启动IMainService", true)]
public sealed class MainBackgroundService : BackgroundService
{
	private CancellationTokenRegistration _applicationStartedRegistration;

	private IAbpLazyServiceProvider LazyServiceProvider { get; }

	private IMainService RunnerService => LazyServiceProvider.LazyGetRequiredService<IMainService>();

	private IHostApplicationLifetime HostApplicationLifetime => LazyServiceProvider.LazyGetRequiredService<IHostApplicationLifetime>();

	private IConfiguration Configuration => LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

	private ILogger Logger => LazyServiceProvider.LazyGetRequiredService<ILogger>();

	public MainBackgroundService(IAbpLazyServiceProvider lazyServiceProvider)
	{
		LazyServiceProvider = lazyServiceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_applicationStartedRegistration = HostApplicationLifetime.ApplicationStarted.Register(delegate
		{
			RunnerService.RunAsync(stoppingToken).ContinueWith(delegate(Task<int> t)
			{
				if (t.IsFaulted)
				{
					AbpLoggerExtensions.LogException(Logger, (Exception)t.Exception, (LogLevel?)null);
				}
				HostApplicationLifetime.StopApplication();
			});
		});
		await Task.CompletedTask;
	}

	public override void Dispose()
	{
		base.Dispose();
		_applicationStartedRegistration.Dispose();
	}
}
