using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Domain.Entities;

namespace BookLendingSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            CreateMap<BorrowRecord, BorrowRecordDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}
