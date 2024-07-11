using System.Threading.Tasks;

namespace Bib5.Abp.Data;

public interface IDbSchemaMigrator
{
	Task MigrateAsync();
}
