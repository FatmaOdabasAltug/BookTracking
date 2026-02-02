namespace BookTracking.Application.Mappings;
using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Domain.Dtos;
using BookTracking.Domain.Entities;
using System.Linq;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity -> DTO
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(a => a.Id).ToList()));
        
        // DTO -> Entity
        // Authors are represented as a list of Guid in DTO; we ignore them here
        // because the service resolves Author entities separately.
        CreateMap<BookDto, Book>()
            .ForMember(dest => dest.Authors, opt => opt.Ignore());

        CreateMap<AuthorDto, Author>();
        CreateMap<Author, AuthorDto>();
        CreateMap<AuditLog, AuditLogDto>();
        CreateMap<AuditLogDto, AuditLog>();

        CreateMap<FilterAuditLogRequestDto, AuditLogFilterParameters>();
    }
}