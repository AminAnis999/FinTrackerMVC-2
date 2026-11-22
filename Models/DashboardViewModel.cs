namespace FinTracker.Models;

public class DashboardViewModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance => TotalIncome - TotalExpenses;
    public List<FinanceTransaction> RecentTransactions { get; set; } = new();
}
