using System;
using Volo.Abp.Domain.Entities;

namespace Bib5.Abp.Sessions;

public class Session : Entity<string>
{
	public virtual string Authority { get; set; } = string.Empty;


	public virtual string? AccessToken { get; set; }

	public virtual string? RefreshToken { get; set; }

	public virtual DateTime? ExpiresAt { get; set; }

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
