namespace Bib5.Abp.Hosting.Console;

public static class Bib5HostingConsoleDbProperties
{
	public const string ConnectionStringName = "Bib5HostingConsole";

	public static string DbTablePrefix { get; set; } = "Bib5Hosting";


	public static string DbSchema { get; set; } = null;

}
