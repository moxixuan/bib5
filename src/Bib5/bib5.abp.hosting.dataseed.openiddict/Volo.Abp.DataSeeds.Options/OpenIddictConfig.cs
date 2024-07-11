using System;

namespace Volo.Abp.DataSeeds.Options;

public class OpenIddictConfig
{
	public ScopeConfig[] Scopes { get; set; } = Array.Empty<ScopeConfig>();


	public ApplicationConfig[] Applications { get; set; } = Array.Empty<ApplicationConfig>();

}
