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
using static Volo.Abp.Hosting.HostingConsts;

namespace Volo.Abp.Hosting;

public static class SerilogConfigurationHelper
{
    public static void Configure(string? appName = null)
    {
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
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName ?? "")
            .Enrich.WithProperty("Environment", text)
            .WriteTo.Async(delegate (LoggerSinkConfiguration c)
        {
            c.Console();
        });
        string path = ApplicationEnvironmentHelper.GetLogRoot(appName);
        if (!ApplicationEnvironmentHelper.IsSetLogRoot())
        {
            path = Path.Combine(path, "logs");
        }

        string logFile = Path.Combine(path, appName.ToLowerInvariant() + ".log");
        loggerConfiguration.WriteTo.Async(delegate (LoggerSinkConfiguration c)
        {
            c.File(logFile, LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: true, 120);
        });

        var seqUrl = configurationRoot["Seq:Url"];
        if (!string.IsNullOrEmpty(seqUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }

        loggerConfiguration.ReadFrom.Configuration(configurationRoot);
        Log.Logger = loggerConfiguration.CreateLogger();
    }
}
