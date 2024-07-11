using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Bib5.Abp.Sessions;

public interface ISessionRepository : IBasicRepository<Session, string>, IBasicRepository<Session>, IReadOnlyBasicRepository<Session>, IRepository, IReadOnlyBasicRepository<Session, string>
{
	Task<Session?> FindByAuthorityAsync(string authority, CancellationToken cancellationToken = default(CancellationToken));
}
