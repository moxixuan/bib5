namespace Volo.Abp.BackgroundWorkers.Hangfire;

public interface IBib5BackgroundWorker
{
	bool EnableDataMigrationEnvironment { get; }

	bool EnableCliEnvironment { get; }
}
