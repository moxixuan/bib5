using System.Collections.Generic;

namespace Volo.Abp.DataSeeds.Options;

public class ApplicationConfig
{
	public string ClientId { get; set; } = string.Empty;


	public string? ClientSecret { get; set; }

	public string ClientType { get; set; } = string.Empty;


	public string ConsentType { get; set; } = string.Empty;


	public string DisplayName { get; set; } = string.Empty;


	public string ClientUri { get; set; } = string.Empty;


	public string LogoUri { get; set; } = string.Empty;


	public HashSet<string> RedirectUris { get; set; } = new HashSet<string>();


	public HashSet<string> PostLogoutRedirectUris { get; set; } = new HashSet<string>();


	public HashSet<string> GrantTypes { get; set; } = new HashSet<string>();


	public HashSet<string> Scopes { get; set; } = new HashSet<string>();


	public HashSet<string> Permissions { get; set; } = new HashSet<string>();

}
