namespace Bib5.Abp.Sessions;

public class SessionConfig
{
	public string SessionKey { get; set; } = "Bib5Auth";


	public bool EnableLocalSynchronization { get; set; }
}
