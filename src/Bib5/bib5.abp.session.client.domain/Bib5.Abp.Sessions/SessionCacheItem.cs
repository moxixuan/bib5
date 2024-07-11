using System;

namespace Bib5.Abp.Sessions;

public class SessionCacheItem
{
	private const string CacheKeyFormat = "session:{0}";

	public string Id { get; set; } = string.Empty;


	public string Authority { get; set; } = string.Empty;


	public string? AccessToken { get; set; }

	public string? RefreshToken { get; set; }

	public DateTime? ExpiresAt { get; set; }

	public string? State { get; set; }

	public static string CalculateKey(string sessionKey)
	{
		return $"session:{sessionKey}";
	}
}
