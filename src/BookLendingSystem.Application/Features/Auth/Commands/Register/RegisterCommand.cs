using BookLendingSystem.Application.DTOs;
using MediatR;

namespace BookLendingSystem.Application.Features.Auth.Commands.Register
{
    
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}