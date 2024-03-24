using Repositories.Implements;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public interface IUnitOfWork
    {
        public ITransactionRepo TransactionRepo { get; }
        public IUserRepo UserRepo { get; }

        public Task<int> SaveChangesAsync();
    }
}
