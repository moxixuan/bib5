using System;
using Volo.Abp.Domain.Entities;

namespace Bib5.Abp.Hosting.Console.Sessions;

public class Session : Entity<string>
{
	public virtual string Authority { get; set; } = string.Empty;


	public virtual string? AccessToken { get; set; }

	public virtual string? RefreshToken { get; set; }

	public virtual DateTime? ExpiresAt { get; set; }

	/// <summary>
	/// 使用授权码方式登陆时的状态码
	/// <para>开始登陆时生成,登陆验证通过后清空</para>
	/// </summary>
	public virtual string? State { get; set; }

	public Session()
	{
	}

	public Session(string id, string authority)
	{
		(this).Id = id;
		Authority = authority;
	}
}
