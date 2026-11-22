using FinTracker.Models;
using FinTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.Controllers;

public class HomeController : Controller
{
    private readonly ITransactionRepository _repository;

    public HomeController(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index() => User.Identity?.IsAuthenticated == true
        ? RedirectToAction(nameof(Dashboard))
        : View("Welcome");

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

        var transactions = await _repository.GetAllAsync(userId);

        var model = new DashboardViewModel
        {
            TotalIncome = transactions.Where(x => x.Type == "Income").Sum(x => x.Amount),
            TotalExpenses = transactions.Where(x => x.Type == "Expense").Sum(x => x.Amount),
            RecentTransactions = transactions.Take(6).ToList()
        };

        return View(model);
    }
}
