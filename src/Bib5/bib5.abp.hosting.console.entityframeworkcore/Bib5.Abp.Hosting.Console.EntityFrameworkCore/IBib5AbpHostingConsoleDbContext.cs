using System;
using Bib5.Abp.Hosting.Console.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Bib5.Abp.Hosting.Console.EntityFrameworkCore;

[ConnectionStringName("Bib5HostingConsole")]
public interface IBib5AbpHostingConsoleDbContext : IEfCoreDbContext, IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbContextPoolable, IResettableService, IAsyncDisposable
{
	DbSet<Session> Sessions { get; }
}
