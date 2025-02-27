using HW.Account.DAL.Data;
using HW.Account.DAL.Data.Entities;
using HW.Common.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HW.Account.BLL.Extensions; 

public static class ConfigureIdentityRoles {
     /// <summary>
    /// Create default roles and administrator user
    /// </summary>
    /// <param name="app"></param>
    public static async Task ConfigureIdentity(this WebApplication app) {
        using var serviceScope = app.Services.CreateScope();

        // Migrate database
        var context = serviceScope.ServiceProvider.GetService<AccountDbContext>();
        
        // Get services
        var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
        if (userManager == null) {
            throw new ArgumentNullException(nameof(userManager));
        }

        var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole<Guid>>>();
        if (roleManager == null) {
            throw new ArgumentNullException(nameof(roleManager));
        }

        // Try to create Roles
        foreach (var roleName in Enum.GetValues(typeof(RoleType))) {
            var strRoleName = roleName.ToString();
            if (strRoleName == null) {
                throw new ArgumentNullException(nameof(roleName), "Some role name is null");
            }

            var role = await roleManager.FindByNameAsync(strRoleName);
            if (role == null) {
                var roleResult =
                    await roleManager.CreateAsync(new IdentityRole<Guid>(strRoleName));
                if (!roleResult.Succeeded) {
                    throw new InvalidOperationException($"Unable to create {strRoleName} role.");
                }

                role = await roleManager.FindByNameAsync(strRoleName);
            }

            if (role == null || role.Name == null) {
                throw new ArgumentNullException(nameof(role), "Can't find role");
            }
        }

        // Get user configuration
        var config = app.Configuration.GetSection("DefaultUsersConfig");

        if (config == null) {
            throw new ArgumentNullException(nameof(config), "DefaultUsersConfig is not defined");
        }

        // Try to create Administrator user
        var adminUser = await userManager.FindByEmailAsync(config["AdminEmail"] ?? throw new ArgumentNullException(
            nameof(config), "AdminEmail is not defined"));
        if (adminUser == null) {
            var userResult = await userManager.CreateAsync(new User {
                FullName = config["AdminFullName"] ?? throw new ArgumentNullException(
                    nameof(config), "AdminFullName is not defined"),
                UserName = config["AdminUserName"] ?? throw new ArgumentNullException(
                    nameof(config), "AdminUserName is not defined"),
                Email = config["AdminEmail"] ?? throw new ArgumentNullException(
                    nameof(config), "AdminEmail is not defined"),
                JoinedAt = DateTime.Now.ToUniversalTime(),
                BirthDate = DateTime.Today.ToUniversalTime()
            }, config["AdminPassword"] ?? throw new ArgumentNullException(
                nameof(config), "AdminPassword is not defined"));
            if (!userResult.Succeeded) {
                throw new InvalidOperationException($"Unable to create administrator user");
            }

            adminUser = await userManager.FindByNameAsync(config["AdminUserName"] ?? throw new ArgumentNullException(
                nameof(config), "AdminUserName is not defined"));
        }

        if (adminUser == null) {
            throw new ArgumentNullException(nameof(adminUser), "Can't find admin user");
        }

        if (!await userManager.IsInRoleAsync(adminUser, ApplicationRoleNames.Administrator)) {
            await userManager.AddToRoleAsync(adminUser, ApplicationRoleNames.Administrator);
        }
    }
}