using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Hosting;

public abstract class BasicMainService : IMainService, ISingletonDependency
{
	protected IAbpLazyServiceProvider LazyServiceProvider { get; }

	protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

	protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>((Func<IServiceProvider, object>)((IServiceProvider provider) => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance));

	protected string[] Args { get; }

	protected BasicMainService(IAbpLazyServiceProvider lazyServiceProvider)
	{
		LazyServiceProvider = lazyServiceProvider;
		Args = Environment.GetCommandLineArgs();
	}

	public abstract Task<int> RunAsync(CancellationToken cancellationToken = default(CancellationToken));
}
