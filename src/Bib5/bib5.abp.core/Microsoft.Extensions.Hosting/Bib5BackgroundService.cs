using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bib5.Abp.Cli;
using Bib5.Abp.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace Microsoft.Extensions.Hosting;

public abstract class Bib5BackgroundService : BackgroundService, IBackgroundService, IHostedService, ISingletonDependency
{
	private readonly List<IBackgroundService> _dependsOn = new List<IBackgroundService>();

	protected bool StartResult { get; set; }

	protected IAbpLazyServiceProvider LazyServiceProvider { get; set; }

	protected ILogger Logger { get; }

	protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

	protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetService<IGuidGenerator>((IGuidGenerator)(object)SimpleGuidGenerator.Instance);

	protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

	protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

	protected IAsyncQueryableExecuter AsyncExecuter => LazyServiceProvider.LazyGetRequiredService<IAsyncQueryableExecuter>();

	public bool IsStarted { get; protected set; }

	public Task<bool> StartTask { get; }

	public bool EnableDataMigrationEnvironment { get; protected set; } = true;


	public bool EnableCliEnvironment { get; protected set; } = true;


	public bool EnableConsoleEnvironment { get; protected set; } = true;


	public IReadOnlyList<IBackgroundService> DependsOn => _dependsOn;

	public Bib5BackgroundService(IAbpLazyServiceProvider lazyServiceProvider)
	{
		LazyServiceProvider = lazyServiceProvider;
		Logger = LazyServiceProvider.LazyGetService<ILogger>((Func<IServiceProvider, object>)((IServiceProvider provider) => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance));
		StartTask = new Task<bool>(() => StartResult);
		foreach (Type item in ((MemberInfo)GetType()).GetCustomAttributes<DependsOnAttribute>(inherit: true).SelectMany((DependsOnAttribute depends) => depends.DependedTypes))
		{
			Check.AssignableTo<IBackgroundService>(item, "dependedType");
			_dependsOn.Add((IBackgroundService)LazyServiceProvider.LazyGetRequiredService(item));
		}
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			Logger.LogInformation("BackgroundService 启动取消 {0}", GetType().Name);
			StartTask.Start();
			await StartTask;
			return;
		}
		if (((IServiceProvider)LazyServiceProvider).IsConsoleEnvironment() && !EnableConsoleEnvironment)
		{
			Logger.LogInformation("BackgroundService 启动忽略 {0}={1} {2}", "EnableConsoleEnvironment", false, GetType().Name);
			StartTask.Start();
			await StartTask;
			return;
		}
		if (AbpDataMigrationEnvironmentExtensions.IsDataMigrationEnvironment((IServiceProvider)LazyServiceProvider) && !EnableDataMigrationEnvironment)
		{
			Logger.LogInformation("BackgroundService 启动忽略 {0}={1} {2}", "EnableDataMigrationEnvironment", false, GetType().Name);
			StartTask.Start();
			await StartTask;
			return;
		}
		if (((IServiceProvider)LazyServiceProvider).IsCliEnvironment() && !EnableCliEnvironment)
		{
			Logger.LogInformation("BackgroundService 启动忽略 {0}={1} {2}", "EnableCliEnvironment", false, GetType().Name);
			StartTask.Start();
			await StartTask;
			return;
		}
		Task.Run(async delegate
		{
			_ = 2;
			try
			{
				await Task.WhenAll(DependsOn.Select((IBackgroundService d) => d.StartTask));
				Logger.LogInformation("BackgroundService 启动开始 {0}", GetType().Name);
				await OnStartAsync(cancellationToken);
				await base.StartAsync(cancellationToken);
				IsStarted = true;
				StartResult = true;
				Logger.LogInformation("BackgroundService 启动成功 {0}", GetType().Name);
			}
			catch (Exception ex)
			{
				StartResult = false;
				Logger.LogInformation("BackgroundService 启动失败 {0}", GetType().Name);
				AbpLoggerExtensions.LogException(Logger, ex, (LogLevel?)null);
				throw;
			}
			finally
			{
				StartTask.Start();
			}
		}, cancellationToken);
	}

	protected virtual async Task OnStartAsync(CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		if ((!((IServiceProvider)LazyServiceProvider).IsConsoleEnvironment() || EnableConsoleEnvironment) && (!AbpDataMigrationEnvironmentExtensions.IsDataMigrationEnvironment((IServiceProvider)LazyServiceProvider) || EnableDataMigrationEnvironment) && (!((IServiceProvider)LazyServiceProvider).IsCliEnvironment() || EnableCliEnvironment))
		{
			IsStarted = false;
			await OnStopAsync(cancellationToken);
			await base.StopAsync(cancellationToken);
			Logger.LogInformation("BackgroundService 退出成功 {0}", GetType().Name);
		}
	}

	protected virtual async Task OnStopAsync(CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}
