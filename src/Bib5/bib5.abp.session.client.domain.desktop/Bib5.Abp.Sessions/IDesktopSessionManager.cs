using System.Threading;
using System.Threading.Tasks;

namespace Bib5.Abp.Sessions;

public interface IDesktopSessionManager : IBasicSessionManager
{
	Task<string> LoginRedirectAsync(string sessionKey, CancellationToken cancellationToken = default(CancellationToken));
}
