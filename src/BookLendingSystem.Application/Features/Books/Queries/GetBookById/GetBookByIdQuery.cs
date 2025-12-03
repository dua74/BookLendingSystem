using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.DTOs;
using MediatR;

namespace BookLendingSystem.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQuery : IRequest<BookDto>
    {
        public int Id { get; set; }
        public GetBookByIdQuery(int id) => Id = id;
    }
}
