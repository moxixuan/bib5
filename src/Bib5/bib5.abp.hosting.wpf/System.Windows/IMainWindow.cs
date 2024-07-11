using Volo.Abp.DependencyInjection;

namespace System.Windows;

public interface IMainWindow : ISingletonDependency
{
	event EventHandler Closed;

	void Show();
}
