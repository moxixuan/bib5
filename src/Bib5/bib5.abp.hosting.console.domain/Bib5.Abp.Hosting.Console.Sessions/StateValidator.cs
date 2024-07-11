using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Bib5.Abp.Hosting.Console.Sessions;

public class StateValidator : DomainService, IStateValidator
{
	protected ISessionRepository SessionRepository => ((DomainService)this).LazyServiceProvider.LazyGetRequiredService<ISessionRepository>();

	public async Task AddStateAsync(string iss, string state, CancellationToken cancellationToken = default(CancellationToken))
	{
		Session session = await SessionRepository.FindByAuthorityAsync(iss, cancellationToken);
		if (session != null)
		{
			session.State = state;
			await ((IBasicRepository<Session>)SessionRepository).UpdateAsync(session, false, cancellationToken);
		}
	}

	public async Task<bool> ValidateAsync(string iss, string state, CancellationToken cancellationToken = default(CancellationToken))
	{
		bool result = false;
		Session session = await SessionRepository.FindByAuthorityAsync(iss, cancellationToken);
		if (session != null)
		{
			result = state.Equals(session.State);
			if (result)
			{
				session.State = null;
				await ((IBasicRepository<Session>)SessionRepository).UpdateAsync(session, false, cancellationToken);
			}
		}
		return result;
	}
}
