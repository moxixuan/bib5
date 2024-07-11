using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Bib5.Abp.Data;

public class NullDbSchemaMigrator : IDbSchemaMigrator, ITransientDependency
{
	public Task MigrateAsync()
	{
		return Task.CompletedTask;
	}
}
