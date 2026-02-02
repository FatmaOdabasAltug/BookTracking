using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;

namespace BookTracking.API.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateBookRequest, BookDto>().ReverseMap();
        CreateMap<UpdateBookRequest, BookDto>().ReverseMap();
    }
}