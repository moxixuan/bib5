using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Bib5.Abp.Sessions;

public interface IServerSessionManager : IBasicSessionManager
{
	ConcurrentDictionary<string, ChangeTokenInfo> SessionTokenLookup { get; }

	Task ClearAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	Task<Session?> LoginAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));

	Task<Session> LoginRedirectCallbackAsync(string sessionKey, string code, string state, CancellationToken cancellationToken = default(CancellationToken));
}
