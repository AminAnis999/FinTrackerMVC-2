using FinTracker.Models;

namespace FinTracker.Repositories;

public interface ITransactionRepository
{
    Task<List<FinanceTransaction>> GetAllAsync(string userId);
    Task<FinanceTransaction?> GetByIdAsync(int id, string userId);
    Task AddAsync(FinanceTransaction item, string userId);
    Task UpdateAsync(FinanceTransaction item, string userId);
    Task DeleteAsync(int id, string userId);
}
