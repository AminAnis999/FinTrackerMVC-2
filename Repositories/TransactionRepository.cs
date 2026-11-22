using Dapper;
using FinTracker.Models;
using Microsoft.Data.SqlClient;

namespace FinTracker.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly string _connectionString;

    public TransactionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");
    }

    public async Task<List<FinanceTransaction>> GetAllAsync(string userId)
    {
        const string sql = @"
            SELECT Id, Title, Type, Amount, Category, TransactionDate, Note
            FROM FinanceTransactions
            WHERE UserId = @UserId
            ORDER BY TransactionDate DESC, Id DESC;";

        await using var connection = new SqlConnection(_connectionString);
        var result = await connection.QueryAsync<FinanceTransaction>(
            sql, new { UserId = userId });

        return result.ToList();
    }

    public async Task<FinanceTransaction?> GetByIdAsync(int id, string userId)
    {
        const string sql = @"
            SELECT Id, Title, Type, Amount, Category, TransactionDate, Note
            FROM FinanceTransactions
            WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<FinanceTransaction>(
            sql, new { Id = id, UserId = userId });
    }

    public async Task AddAsync(FinanceTransaction item, string userId)
    {
        const string sql = @"
            INSERT INTO FinanceTransactions
                (Title, Type, Amount, Category, TransactionDate, Note, UserId)
            VALUES
                (@Title, @Type, @Amount, @Category, @TransactionDate, @Note, @UserId);";

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, new
        {
            item.Title,
            item.Type,
            item.Amount,
            TransactionDate = item.TransactionDate.Date,
            item.Category,
            item.Note,
            UserId = userId
        });
    }

    public async Task UpdateAsync(FinanceTransaction item, string userId)
    {
        const string sql = @"
            UPDATE FinanceTransactions
            SET Title = @Title,
                Type = @Type,
                Amount = @Amount,
                Category = @Category,
                TransactionDate = @TransactionDate,
                Note = @Note
            WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, new
        {
            item.Id,
            item.Title,
            item.Type,
            item.Amount,
            TransactionDate = item.TransactionDate.Date,
            item.Category,
            item.Note,
            UserId = userId
        });
    }

    public async Task DeleteAsync(int id, string userId)
    {
        const string sql = @"
            DELETE FROM FinanceTransactions
            WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
    }
}
