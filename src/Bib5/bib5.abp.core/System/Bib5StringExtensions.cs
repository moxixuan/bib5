namespace System;

public static class Bib5StringExtensions
{
	public static bool Like(this string? input, string? value)
	{
		return input?.Contains(value) ?? false;
	}
}
