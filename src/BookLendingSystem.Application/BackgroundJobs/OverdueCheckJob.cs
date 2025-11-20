using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BookLendingSystem.Application.BackgroundJobs
{
    public class OverdueCheckJob
    {
        private readonly IRepository<BorrowRecord> _repository;
        private readonly ILogger<OverdueCheckJob> _logger;

        public OverdueCheckJob(IRepository<BorrowRecord> repository, ILogger<OverdueCheckJob> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task CheckOverdueBooks()
        {
            _logger.LogInformation("--- Starting Overdue Check Job ---");

            var overdueRecords = await _repository.GetAsync(r => r.ReturnedAt == null && r.DueDate < DateTime.UtcNow);

            if (!overdueRecords.Any())
            {
                _logger.LogInformation("No overdue books found. Great!");
                return;
            }

            foreach (var record in overdueRecords)
            {
               
                _logger.LogWarning("[URGENT] User {UserId} is late returning Book ID {BookId}. Due Date: {DueDate}", record.UserId, record.BookId, record.DueDate);
            }

            _logger.LogInformation("--- Overdue Check Job Finished ---");
        }
    }
}
