using System.Threading;
using System.Threading.Tasks;

namespace Bib5.Abp.Sessions;

public interface IStateValidator
{
	Task AddStateAsync(string iss, string state, CancellationToken cancellationToken = default(CancellationToken));

	Task<bool> ValidateAsync(string iss, string state, CancellationToken cancellationToken = default(CancellationToken));
}
