using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookLendingSystem.Application.Features.Borrowing.Commands.BorrowBook;
using BookLendingSystem.Application.Features.Borrowing.Commands.ReturnBook;
using BookLendingSystem.Application.Features.Borrowing.Queries.GetMyBorrows;

namespace BookLendingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [Authorize] 
    public class BorrowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BorrowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpPost("{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new BorrowBookCommand(userId!, bookId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

      
        [HttpPost("return/{borrowId}")]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new ReturnBookCommand(userId!, borrowId);
            var result = await _mediator.Send(command);
            return Ok(new { Message = "Book returned successfully" });
        }

        
        [HttpGet("my-borrows")]
        public async Task<IActionResult> GetMyBorrows()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetMyBorrowsQuery(userId!);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}