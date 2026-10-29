using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverEgypt.Persistence.Identity;

public static class IdentityRoleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in IdentityRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Could not create the '{roleName}' role: {errors}");
                }
            }
        }
    }
}
