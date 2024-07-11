using System.Threading;
using System.Threading.Tasks;

namespace Bib5.Abp.Sessions;

public interface ILoginProvider
{
	Task<Session> LoginAsync(string sessionKey, string userName, string password, CancellationToken cancellationToken = default(CancellationToken));

	Task<Session> LoginAsync(string sessionKey, string refreshToken, CancellationToken cancellationToken = default(CancellationToken));

	Task<string> LoginRedirectAsync(string sessionKey, CancellationToken cancellationToken);

	Task<Session> LoginRedirectCallbackAsync(string sessionKey, string code, string state, CancellationToken cancellationToken = default(CancellationToken));
}
