using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR.Client;

public static class HubConnectionExpansions
{
	public static async Task<bool> StartAsync(this HubConnection hubConnection, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default(CancellationToken))
	{
		RetryContext retryContext = new RetryContext();
		DateTime? retryStartTime = null;
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				await hubConnection.StartAsync(cancellationToken);
				return true;
			}
			catch (Exception retryReason)
			{
				retryContext.RetryReason = retryReason;
				if (!retryStartTime.HasValue)
				{
					retryStartTime = DateTime.Now;
				}
				else
				{
					long previousRetryCount = retryContext.PreviousRetryCount;
					retryContext.PreviousRetryCount = previousRetryCount + 1;
				}
				retryContext.ElapsedTime = DateTime.Now - retryStartTime.Value;
				if (!retryPolicy.NextRetryDelay(retryContext).HasValue)
				{
					return false;
				}
				await Task.Delay(retryPolicy.NextRetryDelay(retryContext).Value, cancellationToken);
			}
		}
		return false;
	}
}
