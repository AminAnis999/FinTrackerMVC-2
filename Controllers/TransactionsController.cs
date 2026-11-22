using FinTracker.Models;
using FinTracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionRepository _repository;

    private string CurrentUserId =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

    public TransactionsController(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index() =>
        View(await _repository.GetAllAsync(CurrentUserId));

    [HttpGet]
    public IActionResult Create() =>
        View(new FinanceTransaction { TransactionDate = DateTime.Today });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FinanceTransaction model)
    {
        ValidateType(model);

        if (!ModelState.IsValid)
            return View(model);

        await _repository.AddAsync(model, CurrentUserId);
        TempData["Success"] = "Transaction added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _repository.GetByIdAsync(id, CurrentUserId);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FinanceTransaction model)
    {
        if (id != model.Id)
            return BadRequest();

        ValidateType(model);

        if (!ModelState.IsValid)
            return View(model);

        var existing = await _repository.GetByIdAsync(id, CurrentUserId);
        if (existing == null)
            return NotFound();

        await _repository.UpdateAsync(model, CurrentUserId);
        TempData["Success"] = "Transaction updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id, CurrentUserId);
        TempData["Success"] = "Transaction deleted.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateType(FinanceTransaction model)
    {
        if (model.Type != "Income" && model.Type != "Expense")
            ModelState.AddModelError(nameof(model.Type), "Choose Income or Expense.");
    }
}
