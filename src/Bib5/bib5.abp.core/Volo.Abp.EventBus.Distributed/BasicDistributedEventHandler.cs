using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace Volo.Abp.EventBus.Distributed;

public abstract class BasicDistributedEventHandler<TEvent> : IDistributedEventHandler<TEvent>, IEventHandler, ITransientDependency
{
	protected IAbpLazyServiceProvider LazyServiceProvider { get; }

	protected ILogger Logger { get; }

	protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();

	protected BasicDistributedEventHandler(IAbpLazyServiceProvider lazyServiceProvider)
	{
		LazyServiceProvider = lazyServiceProvider;
		Logger = LazyServiceProvider.LazyGetService<ILogger>((Func<IServiceProvider, object>)((IServiceProvider provider) => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>().CreateLogger(GetType().FullName) ?? NullLogger.Instance));
	}

	public abstract Task HandleEventAsync(TEvent eventData);
}
