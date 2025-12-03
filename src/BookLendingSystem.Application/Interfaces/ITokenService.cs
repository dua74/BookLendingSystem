using BookLendingSystem.Domain.Entities;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
    }
}