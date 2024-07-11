using System;
using System.Collections.Generic;

namespace Volo.Abp.DataSeeds.Options;

public class ScopeConfig
{
	public string Name { get; set; } = string.Empty;


	public string? DisplayName { get; set; }

	public string? Description { get; set; }

	public HashSet<string> Resources { get; set; } = new HashSet<string>(StringComparer.Ordinal);

}
