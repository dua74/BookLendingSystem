using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using MediatR;

namespace BookLendingSystem.Application.Features.Borrowing.Queries.GetMyBorrows
{
    public class GetMyBorrowsQueryHandler : IRequestHandler<GetMyBorrowsQuery, IEnumerable<BorrowRecordDto>>
    {
        private readonly IRepository<BorrowRecord> _borrowRepository;
        private readonly IMapper _mapper;

        public GetMyBorrowsQueryHandler(IRepository<BorrowRecord> borrowRepository, IMapper mapper)
        {
            _borrowRepository = borrowRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BorrowRecordDto>> Handle(GetMyBorrowsQuery request, CancellationToken cancellationToken)
        {
         
            var records = await _borrowRepository.GetAsync(
                b => b.UserId == request.UserId,
                b => b.Book, b => b.User
            );

            return _mapper.Map<IEnumerable<BorrowRecordDto>>(records);
        }
    }
}
