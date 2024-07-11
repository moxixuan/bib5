namespace Bib5.Abp.Hosting.Console;

public static class Bib5SessionDbProperties
{
	public const string ConnectionStringName = "Bib5Session";

	public static string DbTablePrefix { get; set; } = "Bib5";


	public static string DbSchema { get; set; } = null;

}
