using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models;

public class FinanceTransaction
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = "";

    [Required]
    public string Type { get; set; } = "Expense"; // Expense or Income

    [Range(0.01, 100000000)]
    public decimal Amount { get; set; }

    [Required]
    public string Category { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [StringLength(300)]
    public string? Note { get; set; }
}
