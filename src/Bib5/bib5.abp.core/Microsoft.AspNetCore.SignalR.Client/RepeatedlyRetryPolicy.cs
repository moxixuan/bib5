using System;

namespace Microsoft.AspNetCore.SignalR.Client;

public class RepeatedlyRetryPolicy : IRetryPolicy
{
	public TimeSpan RetryTimeSpan { get; set; } = TimeSpan.FromSeconds(2.0);


	public RepeatedlyRetryPolicy(TimeSpan? retryTimeSpan = null)
	{
		if (retryTimeSpan.HasValue)
		{
			RetryTimeSpan = retryTimeSpan.Value;
		}
	}

	public TimeSpan? NextRetryDelay(RetryContext retryContext)
	{
		if (retryContext.PreviousRetryCount == 0L)
		{
			return TimeSpan.Zero;
		}
		return RetryTimeSpan;
	}
}
