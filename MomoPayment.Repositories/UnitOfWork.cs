using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExagenContext _context;

        private readonly ITransactionRepo _transactionRepo;
        private readonly IUserRepo _userRepo;

        public UnitOfWork(ExagenContext context, ITransactionRepo transactionRepo, IUserRepo userRepo)
        {
            _context = context;
            _transactionRepo = transactionRepo;
            _userRepo = userRepo;
        }

        public ITransactionRepo TransactionRepo => _transactionRepo;
        public IUserRepo UserRepo => _userRepo;

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }
    }
}
