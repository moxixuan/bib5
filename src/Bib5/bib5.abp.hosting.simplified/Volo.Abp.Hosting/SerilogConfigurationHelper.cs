using System;
using System.IO;
using System.Text;
using Bib5.Abp.Hosting.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Settings.Configuration;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;

namespace Volo.Abp.Hosting;

public static class SerilogConfigurationHelper
{
    public static void Configure(string? appName = null)
    {
        //IL_0090: Unknown result type (might be due to invalid IL or missing references)
        ConfigurationManager configurationManager = new ConfigurationManager();
        configurationManager.AddJsonFile("appsettings.json");
        configurationManager.AddEnvironmentVariables("ASPNETCORE_");
        string text = configurationManager[HostDefaults.EnvironmentKey];
        if (File.Exists("appsettings." + text + ".json"))
        {
            configurationManager.AddJsonFile("appsettings." + text + ".json");
        }
        IConfigurationRoot configurationRoot = configurationManager.AddEnvironmentVariables().AddCommandLine(Environment.GetCommandLineArgs()).Build();
        if (appName == null)
        {
            appName = configurationRoot["App:AppName"];
        }
        Check.NotNull<string>(appName, "appName");
        LoggerConfiguration obj = LoggerConfigurationAsyncExtensions.Async(new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext().Enrich.WithProperty("Application", (object)(appName ?? ""), false).WriteTo, (Action<LoggerSinkConfiguration>)delegate (LoggerSinkConfiguration c)
        {
            ConsoleLoggerConfigurationExtensions.Console(c, (LogEventLevel)0, "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", (IFormatProvider)null, (LoggingLevelSwitch)null, (LogEventLevel?)null, (ConsoleTheme)null, false, (object)null);
        }, 10000, false);
        string path = ApplicationEnvironmentHelper.GetLogRoot(appName);
        if (!ApplicationEnvironmentHelper.IsSetLogRoot())
        {
            path = Path.Combine(path, "logs");
        }
        string logFile = Path.Combine(path, appName.ToLowerInvariant() + ".log");
        LoggerConfigurationAsyncExtensions.Async(obj.WriteTo, (Action<LoggerSinkConfiguration>)delegate (LoggerSinkConfiguration c)
        {
            FileLoggerConfigurationExtensions.File(
                c,
                logFile,
                (LogEventLevel)0,
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                (IFormatProvider)null,
                (long?)1073741824L,
                (LoggingLevelSwitch)null,
                false,
                false,
                (TimeSpan?)null,
                RollingInterval.Day,
                false,
                (int?)120,
                (Encoding)null,
                (FileLifecycleHooks)null,
                (TimeSpan?)null);
        }, 10000, false);
        ConfigurationLoggerConfigurationExtensions.Configuration(obj.ReadFrom, (IConfiguration)configurationRoot, (ConfigurationReaderOptions)null);
        Log.Logger = (ILogger)(object)obj.CreateLogger();
    }
}
