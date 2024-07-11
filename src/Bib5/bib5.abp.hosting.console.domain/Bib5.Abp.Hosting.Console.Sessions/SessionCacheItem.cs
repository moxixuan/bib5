using System;

namespace Bib5.Abp.Hosting.Console.Sessions;

public class SessionCacheItem
{
	private const string CacheKeyFormat = "session:{0}";

	public string Id { get; set; } = string.Empty;


	public string Authority { get; set; } = string.Empty;


	public string? AccessToken { get; set; }

	public string? RefreshToken { get; set; }

	public DateTime? ExpiresAt { get; set; }

	/// <summary>
	/// 使用授权码方式登陆时的状态码
	/// <para>开始登陆时生成,登陆验证通过后清空</para>
	/// </summary>
	public string? State { get; set; }

	public static string CalculateKey(string sessionKey)
	{
		return $"session:{sessionKey}";
	}
}
