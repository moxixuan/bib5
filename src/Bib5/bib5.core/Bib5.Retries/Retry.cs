using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Bib5.Retries;

public static class Retry
{
	public static void Execute(Func<bool> func, int maxRetryTimes = 2, int retryInterval = 0)
	{
		Check.NotNull<Func<bool>>(func, "func");
		CheckEx.RangeMin("maxRetryTimes", maxRetryTimes, 1m);
		int num = 1;
		bool flag = false;
		bool flag2;
		do
		{
			if (flag)
			{
				Thread.Sleep(retryInterval);
			}
			flag2 = func();
			flag = true;
		}
		while ((num == -1 || maxRetryTimes > num++) && !flag2);
	}

	public static async Task ExecuteAsync(Func<Task<bool>> func, int maxRetryTimes = 2, int retryInterval = 0)
	{
		Check.NotNull<Func<Task<bool>>>(func, "func");
		CheckEx.RangeMin("maxRetryTimes", maxRetryTimes, 1m);
		int retryTimes = 0;
		bool flag = false;
		bool flag2;
		do
		{
			if (flag)
			{
				await Task.Delay(retryInterval);
			}
			flag2 = await func();
			flag = true;
		}
		while (maxRetryTimes > retryTimes++ && !flag2);
	}

	public static TReturn Execute<TReturn>(Func<TReturn> func, Func<TReturn, bool> succeedCondition, int maxRetryTimes = 2, int retryInterval = 0)
	{
		Check.NotNull<Func<TReturn>>(func, "func");
		Check.NotNull<Func<TReturn, bool>>(succeedCondition, "succeedCondition");
		CheckEx.RangeMin("maxRetryTimes", maxRetryTimes, 1m);
		int num = 1;
		bool flag = false;
		TReturn val;
		bool flag2;
		do
		{
			if (flag)
			{
				Thread.Sleep(retryInterval);
			}
			val = func();
			flag2 = succeedCondition(val);
			flag = true;
		}
		while (maxRetryTimes > num++ && !flag2);
		return val;
	}

	public static async Task<TReturn> ExecuteAsync<TReturn>(Func<Task<TReturn>> func, Func<TReturn, bool> succeedCondition, int maxRetryTimes = 2, int retryInterval = 0)
	{
		Check.NotNull<Func<Task<TReturn>>>(func, "func");
		Check.NotNull<Func<TReturn, bool>>(succeedCondition, "succeedCondition");
		CheckEx.RangeMin("maxRetryTimes", maxRetryTimes, 1m);
		int retryTimes = 0;
		bool flag = false;
		TReturn val;
		bool flag2;
		do
		{
			if (flag)
			{
				await Task.Delay(retryInterval);
				Console.Write($"重试次数:{retryTimes}");
			}
			val = await func();
			flag2 = succeedCondition(val);
			flag = true;
		}
		while (maxRetryTimes > retryTimes++ && !flag2);
		return val;
	}
}
