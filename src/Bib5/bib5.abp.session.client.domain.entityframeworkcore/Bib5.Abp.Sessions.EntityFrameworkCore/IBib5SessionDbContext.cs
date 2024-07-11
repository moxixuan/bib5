using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Bib5.Abp.Sessions.EntityFrameworkCore;

[ConnectionStringName("Bib5Session")]
public interface IBib5SessionDbContext : IEfCoreDbContext, IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbContextPoolable, IResettableService, IAsyncDisposable
{
	DbSet<Session> Sessions { get; }
}
