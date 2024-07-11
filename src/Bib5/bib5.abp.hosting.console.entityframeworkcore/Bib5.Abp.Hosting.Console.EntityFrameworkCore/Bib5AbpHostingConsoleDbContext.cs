using System;
using Bib5.Abp.Hosting.Console.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Bib5.Abp.Hosting.Console.EntityFrameworkCore;

[ConnectionStringName("Bib5HostingConsole")]
public class Bib5AbpHostingConsoleDbContext : AbpDbContext<Bib5AbpHostingConsoleDbContext>, IBib5AbpHostingConsoleDbContext, IEfCoreDbContext, IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbContextPoolable, IResettableService, IAsyncDisposable, ISettingManagementDbContext
{
	public DbSet<Session> Sessions { get; set; }

	public DbSet<Setting> Settings { get; set; }

	public DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; set; }

	public Bib5AbpHostingConsoleDbContext(DbContextOptions<Bib5AbpHostingConsoleDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		SettingManagementDbContextModelBuilderExtensions.ConfigureSettingManagement(builder);
		builder.ConfigureBib5AbpHostingConsole();
	}
}
