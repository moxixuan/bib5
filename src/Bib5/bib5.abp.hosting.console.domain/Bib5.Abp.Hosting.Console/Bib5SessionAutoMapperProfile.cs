using AutoMapper;
using Bib5.Abp.Hosting.Console.Sessions;

namespace Bib5.Abp.Hosting.Console;

public class Bib5SessionAutoMapperProfile : Profile
{
	public Bib5SessionAutoMapperProfile()
	{
		((Profile)this).CreateMap<Session, SessionCacheItem>();
	}
}
