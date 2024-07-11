using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Bib5.Abp.Sessions;

public interface IBasicSessionManager
{
	Task<Session?> GetAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	Task<Session> SetAsync(Session session, CancellationToken cancellationToken = default(CancellationToken));

	Task<bool> IsValidAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	Task<SessionStatus> GetSessionStatusAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken = default(CancellationToken));

	Task LogoutAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	Task ReloadAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	IChangeToken GetAccessTokenChangeToken(string sessionKey);
}
