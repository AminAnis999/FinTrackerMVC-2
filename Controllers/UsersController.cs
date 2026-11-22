using FinTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync();

        return View(users);
    }
}
