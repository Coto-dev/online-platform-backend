using HW.Common.Interfaces;
using HW.FileManager.BL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HW.FileManager.BL.Extensions; 

public static class ConfigureServicesDependencies {
    public static IServiceCollection AddFileServiceDependencies(this IServiceCollection services) {
        services.AddScoped<IFileService, FileService>();
        return services;
    }
}