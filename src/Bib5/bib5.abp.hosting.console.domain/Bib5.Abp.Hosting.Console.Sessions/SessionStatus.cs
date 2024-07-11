namespace Bib5.Abp.Hosting.Console.Sessions;

public enum SessionStatus
{
	/// <summary>
	/// 未登陆
	/// </summary>
	NotLoggedIn,
	/// <summary>
	/// 已登陆
	/// </summary>
	Vaild,
	/// <summary>
	/// 已失效
	/// </summary>
	Expires
}
