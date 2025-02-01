using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class IdentityAppDbContext : IdentityDbContext
{
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
    {
    }
}

public static class SeedData
{
    public static async Task Seed(this IApplicationBuilder applicationBuilder)
    {
        var service = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

        var applicationUserManagement = service.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var applicationRoleManagement = service.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var adminRole = new IdentityRole("Admin");
        var coustomerRole = new IdentityRole("Customer");

        if (!applicationRoleManagement.Roles.Any(c => c.Name == "Admin"))
            applicationRoleManagement.CreateAsync(adminRole).Wait();

        if (!applicationRoleManagement.Roles.Any(c => c.Name == "Customer"))
            applicationRoleManagement.CreateAsync(coustomerRole).Wait();

        if (applicationUserManagement != null && !applicationUserManagement.Users.Any())
        {
            foreach (var item in SeedUsers())
            {
                var result =await applicationUserManagement.CreateAsync(item, "123456Aa@@");
                if (item.UserName == "reza")
                {
                    var identityResult =await applicationUserManagement.AddToRoleAsync(item, "Admin");
                }
                else
                {
                    var identityResult = await applicationUserManagement.AddToRoleAsync(item, "Customer");
                }
            }
        }
    }

    private static List<IdentityUser> SeedUsers()
    {
        var list = new List<IdentityUser>()
        {
            new IdentityUser()
            {
                UserName = "reza",
                Email = "Reza@gmail.com",
                EmailConfirmed = true,
            },
            new IdentityUser()
            {
                UserName = "mohammad",
                Email = "Mohammad@gmail.com",
                EmailConfirmed = true,
            },
            new IdentityUser()
            {
                UserName = "mehdi",
                Email = "Mehdi@gmail.com",
                EmailConfirmed = true,
            }
        };
        return list;
    }
}