using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Bib5.Abp.Data;

public class DefaultDbMigrationService : ITransientDependency
{
	private readonly IDataSeeder _dataSeeder;

	private readonly IEnumerable<IDbSchemaMigrator> _dbSchemaMigrators;

	public ILogger<DefaultDbMigrationService> Logger { get; set; } = NullLogger<DefaultDbMigrationService>.Instance;


	public DefaultDbMigrationService(IDataSeeder dataSeeder, IEnumerable<IDbSchemaMigrator> dbSchemaMigrators):base()
	{
		_dataSeeder = dataSeeder;
		_dbSchemaMigrators = dbSchemaMigrators;
		//base._002Ector();
	}

	public async Task MigrateAsync()
	{
		await MigrateDatabaseSchemaAsync();
		await SeedDataAsync();
	}

	private async Task MigrateDatabaseSchemaAsync()
	{
		Logger.LogInformation("Migrating schema for host database...");
		foreach (IDbSchemaMigrator dbSchemaMigrator in _dbSchemaMigrators)
		{
			await dbSchemaMigrator.MigrateAsync();
		}
	}

	private async Task SeedDataAsync()
	{
		Logger.LogInformation("Executing host database seed...");
		await _dataSeeder.SeedAsync(new DataSeedContext((Guid?)null));
	}
}
