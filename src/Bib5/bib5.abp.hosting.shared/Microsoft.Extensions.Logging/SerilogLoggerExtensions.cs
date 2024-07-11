using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;
using Volo.Abp;

namespace Microsoft.Extensions.Logging;

public static class SerilogLoggerExtensions
{
	public static ILoggingBuilder AddBib5Serilog(this ILoggingBuilder builder, Serilog.ILogger? logger = null, bool dispose = false)
	{
        //IL_003e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0048: Expected O, but got Unknown
        Serilog.ILogger logger2 = logger;
		Check.NotNull<ILoggingBuilder>(builder, "builder");
		if (dispose)
		{
			builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>((Func<IServiceProvider, SerilogLoggerProvider>)((IServiceProvider services) => new SerilogLoggerProvider(logger2, true)));
		}
		else
		{
			builder.AddProvider((ILoggerProvider)new SerilogLoggerProvider(logger2, false));
		}
		return builder;
	}
}
