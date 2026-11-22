using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FinTracker.Data;

public static class IdentitySeeder
{
    public const string AdminClaimType = "FinTrackerAdmin";

    public static async Task SeedAdminAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];

        // Admin creation is optional. Never place an admin password in source control.
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create the configured FinTracker admin account: {errors}");
            }
        }

        var claims = await userManager.GetClaimsAsync(user);
        if (!claims.Any(claim =>
            claim.Type == AdminClaimType && claim.Value == "true"))
        {
            var claimResult = await userManager.AddClaimAsync(
                user, new Claim(AdminClaimType, "true"));

            if (!claimResult.Succeeded)
            {
                var errors = string.Join("; ",
                    claimResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Could not assign the FinTracker admin claim: {errors}");
            }
        }
    }
}
