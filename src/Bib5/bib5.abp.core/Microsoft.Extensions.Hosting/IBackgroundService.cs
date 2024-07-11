using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting;

public interface IBackgroundService : IHostedService
{
	bool IsStarted { get; }

	Task<bool> StartTask { get; }

	bool EnableDataMigrationEnvironment { get; }

	bool EnableCliEnvironment { get; }

	bool EnableConsoleEnvironment { get; }

	IReadOnlyList<IBackgroundService> DependsOn { get; }
}
