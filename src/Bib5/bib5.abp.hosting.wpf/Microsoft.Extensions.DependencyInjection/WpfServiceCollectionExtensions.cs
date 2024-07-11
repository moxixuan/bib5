using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class WpfServiceCollectionExtensions
{
	public static IServiceCollection AddBib5NotifyIcon(this IServiceCollection services, NotifyIcon notifyIcon)
	{
		services.TryAddSingleton(notifyIcon);
		return services;
	}
}
