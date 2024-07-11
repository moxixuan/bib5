using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace Microsoft.DotNet.Builder;

public static class AbpApplicationBuilderExtensions
{
	public static async Task InitializeApplicationAsync(this IHost app)
	{
		Check.NotNull<IHost>(app, "app");
		app.Services.GetRequiredService<ObjectAccessor<IHost>>().Value = app;
		IAbpApplicationWithExternalServiceProvider application = app.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>();
		IHostApplicationLifetime requiredService = app.Services.GetRequiredService<IHostApplicationLifetime>();
		requiredService.ApplicationStopping.Register(delegate
		{
			AsyncHelper.RunSync((Func<Task>)(() => ((IAbpApplication)application).ShutdownAsync()));
		});
		requiredService.ApplicationStopped.Register(delegate
		{
			((IDisposable)application).Dispose();
		});
		await application.InitializeAsync(app.Services);
	}
}
