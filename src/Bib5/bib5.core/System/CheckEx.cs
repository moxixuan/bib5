using System.Diagnostics;

namespace System;

[DebuggerStepThrough]
public static class CheckEx
{
	public static void Range(string paramName, decimal value, decimal min, decimal max, bool includeMin = true, bool includeMax = true)
	{
		if (includeMin && includeMax && (value < min || value > max))
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;[{min},{max}]");
		}
		if (includeMin && !includeMax && (value < min || value >= max))
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;[{min},{max})");
		}
		if (!includeMin && includeMax && (value <= min || value > max))
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;({min},{max}]");
		}
		if (!includeMin && !includeMax && (value <= min || value >= max))
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;({min},{max})");
		}
	}

	public static void RangeMin(string paramName, decimal value, decimal min, bool includeMin = true)
	{
		if (includeMin && value < min)
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;[{min},无穷大]");
		}
		if (!includeMin && value <= min)
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;({min},无穷大]");
		}
	}

	public static void RangeMax(string paramName, decimal value, decimal max, bool includeMax = true)
	{
		if (includeMax && value > max)
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;[无穷小,{max}]");
		}
		if (!includeMax && value >= max)
		{
			throw new ArgumentOutOfRangeException(paramName, value, $"{value}超出有效范围;(无穷小,{max}]");
		}
	}
}
