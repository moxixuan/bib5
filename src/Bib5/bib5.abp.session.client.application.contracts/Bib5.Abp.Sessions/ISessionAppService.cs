using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Bib5.Abp.Sessions;

public interface ISessionAppService : IApplicationService, IRemoteService
{
	Task LoginNotifyAsync(CancellationToken cancellationToken = default(CancellationToken));

	Task LogoutNotifyAsync(CancellationToken cancellationToken = default(CancellationToken));
}
