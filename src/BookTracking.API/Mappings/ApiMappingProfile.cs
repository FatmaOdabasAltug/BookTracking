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
        CreateMap<BookDto, BookResponse>();

        CreateMap<CreateAuthorRequest, AuthorDto>().ReverseMap();
        CreateMap<UpdateAuthorRequest, AuthorDto>().ReverseMap();
        CreateMap<AuthorDto, AuthorResponse>();

        CreateMap<FilterAuditLogRequest, FilterAuditLogRequestDto>();
        CreateMap<AuditLogDto, AuditLogResponse>();
    }
}