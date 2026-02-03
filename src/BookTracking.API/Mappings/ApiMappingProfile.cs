using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;

namespace BookTracking.API.Mappings;
public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<CreateBookRequest, BookDto>().ReverseMap();
        CreateMap<UpdateBookRequest, BookDto>().ReverseMap();
        CreateMap<BookDto, BookResponse>()
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.AuthorDetails));

        CreateMap<CreateAuthorRequest, AuthorDto>().ReverseMap();
        CreateMap<UpdateAuthorRequest, AuthorDto>().ReverseMap();
        CreateMap<AuthorDto, AuthorResponse>()
            .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.BookDetails));
        
        CreateMap<AuthorSummaryDto, AuthorSummaryResponse>();
        CreateMap<BookSummaryDto, BookSummaryResponse>();

        CreateMap<FilterAuditLogRequest, AuditLogFilterCriteriaDto>();
        CreateMap<AuditLogDto, AuditLogResponse>();
        CreateMap<GroupedAuditLogDto, GroupedAuditLogResponse>();
    }
}