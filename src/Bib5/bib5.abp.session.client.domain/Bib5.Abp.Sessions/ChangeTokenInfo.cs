using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Bib5.Abp.Sessions;

public readonly struct ChangeTokenInfo
{
	public CancellationTokenSource TokenSource { get; }

	public CancellationChangeToken ChangeToken { get; }

	public ChangeTokenInfo(CancellationTokenSource tokenSource, CancellationChangeToken changeToken)
	{
		TokenSource = tokenSource;
		ChangeToken = changeToken;
	}
}
