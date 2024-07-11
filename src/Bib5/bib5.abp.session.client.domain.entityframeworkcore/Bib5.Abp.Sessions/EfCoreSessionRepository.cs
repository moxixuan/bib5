using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bib5.Abp.Sessions.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Bib5.Abp.Sessions;

public class EfCoreSessionRepository : EfCoreRepository<IBib5SessionDbContext, Session, string>, ISessionRepository, IBasicRepository<Session, string>, IBasicRepository<Session>, IReadOnlyBasicRepository<Session>, IRepository, IReadOnlyBasicRepository<Session, string>
{
	public EfCoreSessionRepository(IDbContextProvider<IBib5SessionDbContext> dbContextProvider)
		: base(dbContextProvider)
	{
	}

	public async Task<Session?> FindByAuthorityAsync(string authority, CancellationToken cancellationToken = default(CancellationToken))
	{
		string authority2 = authority;
		Check.NotNullOrEmpty(authority2, "authority", int.MaxValue, 0);
		return await ((RepositoryBase<Session>)(object)this).FindAsync((Expression<Func<Session, bool>>)((Session session) => authority2.Equals(session.Authority)), true, cancellationToken);
	}
}
