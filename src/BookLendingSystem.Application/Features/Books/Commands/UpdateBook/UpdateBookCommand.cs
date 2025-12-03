using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.DTOs;
using MediatR;

namespace BookLendingSystem.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommand : UpdateBookDto, IRequest<bool>
    {
        public int Id { get; set; } 
    }
}
