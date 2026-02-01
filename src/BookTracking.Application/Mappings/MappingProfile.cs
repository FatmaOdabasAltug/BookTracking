namespace BookTracking.Application.Mappings;
using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity -> DTO
        CreateMap<Book, BookDto>();
        
        // DTO -> Entity
        CreateMap<BookDto, Book>();
        CreateMap<AuthorDto, Author>();
        CreateMap<Author, AuthorDto>();
        CreateMap<AuditLog, AuditLogDto>();
        CreateMap<AuditLogDto, AuditLog>();
    }
}