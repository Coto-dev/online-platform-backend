using HW.Account.BLL.Services;
using HW.Account.DAL.Data;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HW.Account.BLL.Extensions; 

/// <summary>
/// Service dependency extension
/// </summary>
public static class ServiceDependencyExtension {
    /// <summary>
    /// Add BackendAPI BL service dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAccountBlServiceDependencies(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<AccountDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("AccountDatabase")));
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        return services;
    }
}