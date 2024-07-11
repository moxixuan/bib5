using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Bib5.Abp.Sessions.EntityFrameworkCore;

[ConnectionStringName("Bib5Session")]
public class Bib5SessionDbContext : AbpDbContext<Bib5SessionDbContext>, IBib5SessionDbContext, IEfCoreDbContext, IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbContextPoolable, IResettableService, IAsyncDisposable
{
	public DbSet<Session> Sessions { get; set; }

	public Bib5SessionDbContext(DbContextOptions<Bib5SessionDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ConfigureBib5Session();
	}
}
