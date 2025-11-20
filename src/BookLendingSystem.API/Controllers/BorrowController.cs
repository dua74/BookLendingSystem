using System.Security.Claims;
using BookLendingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLendingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _borrowService;

        public BorrowController(IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        [HttpPost("{bookId}")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            try
            {
               
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _borrowService.BorrowBookAsync(userId!, bookId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{borrowId}")]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _borrowService.ReturnBookAsync(userId!, borrowId);

                if (!result) return BadRequest("Cannot return this book (maybe it's not yours or invalid ID).");

                return Ok("Book returned successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-borrows")]
        public async Task<IActionResult> GetMyBorrows()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrows = await _borrowService.GetMyBorrowsAsync(userId!);
            return Ok(borrows);
        }
    }
}
