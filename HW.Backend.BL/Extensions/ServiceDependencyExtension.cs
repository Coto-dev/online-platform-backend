using HW.Backend.BL.Services;
using HW.Backend.DAL.Data;
using HW.Common.Interfaces;
using HW.FileManager.BL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HW.Backend.BL.Extensions; 

public static class ServiceDependencyExtension {
    public static IServiceCollection AddBackendServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<BackendDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("BackendDatabase")));
        services.AddScoped<IModuleManagerService, ModuleManagerService>();
        services.AddScoped<IChapterService, ChapterService>();
        services.AddScoped<ICheckPermissionService, CheckPermissionService>();
        services.AddScoped<IModuleStudentService, ModuleStudentService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IActivityService, ActivityService>();

        return services;
    }
}