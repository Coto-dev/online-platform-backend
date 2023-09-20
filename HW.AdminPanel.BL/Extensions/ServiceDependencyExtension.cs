using HW.Account.DAL.Data;
using HW.AdminPanel.BL.Services;
using HW.Backend.DAL.Data;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HW.AdminPanel.BL.Extensions;

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension
{
    /// <summary>
    /// Add AdminPanel BL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAdminPanelBlServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("AccountDatabase")));

        services.AddDbContext<BackendDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("BackendDatabase")));

        services.AddScoped<IAdminPanelService, AdminPanelService>();

        return services;
    }
}