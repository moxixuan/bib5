using System;

namespace Bib5.Abp.Sessions;

public static class SessionExtensions
{
	public static bool IsExpires(this Session? session)
	{
		if (session == null)
		{
			return true;
		}
		if (!session.ExpiresAt.HasValue)
		{
			return true;
		}
		return session.ExpiresAt <= DateTime.Now;
	}

	public static long ExpiresIn(this Session? session)
	{
		if (session == null)
		{
			return long.MinValue;
		}
		if (!session.ExpiresAt.HasValue)
		{
			return long.MinValue;
		}
		return (long)(session.ExpiresAt.Value - DateTime.Now).TotalSeconds;
	}
}
