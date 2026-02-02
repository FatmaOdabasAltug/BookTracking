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
    }
}