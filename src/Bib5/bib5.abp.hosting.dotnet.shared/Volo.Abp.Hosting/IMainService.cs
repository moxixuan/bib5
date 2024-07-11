using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Hosting;

public interface IMainService : ISingletonDependency
{
	Task<int> RunAsync(CancellationToken cancellationToken = default(CancellationToken));
}
