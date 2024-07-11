using AutoMapper;

namespace Bib5.Abp.Sessions;

public class Bib5SessionAutoMapperProfile : Profile
{
	public Bib5SessionAutoMapperProfile()
	{
		((Profile)this).CreateMap<Session, SessionCacheItem>().ReverseMap();
	}
}
