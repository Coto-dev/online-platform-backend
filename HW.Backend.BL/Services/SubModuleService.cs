using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HW.Backend.BL.Services; 

public class SubModuleService : ISubModuleService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;
    private readonly IFileService _fileService;

    public SubModuleService(BackendDbContext dbContext, IFileService fileService, ILogger<ModuleManagerService> logger) {
        _dbContext = dbContext;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task EditSubModulesOrder(List<Guid> orderedSubModules, Guid moduleId) {
        var duplicates = orderedSubModules.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new BadRequestException("There are duplicates: " + string.Join(", ", duplicates.Select(x => x)));
        var module = await _dbContext.Modules
            .Include(m=>m.SubModules)
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (module.SubModules.IsNullOrEmpty() || module.SubModules!.All(c => c.ArchivedAt.HasValue))
            throw new ConflictException("There are no existing sub modules");
        var missingSubs = module.SubModules!
            .Where(s=>!s.ArchivedAt.HasValue)
            .Select(o=>o.Id)
            .Except(orderedSubModules)
            .ToList();
        if (missingSubs.Any())
            throw new ConflictException("These sub modules are missing: " 
                                        + string.Join(", ", missingSubs.Select(x => x)));
        var notExistingSubs = orderedSubModules
            .Except(module.SubModules!
                .Where(s=>!s.ArchivedAt.HasValue)
                .Select(o=>o.Id)
                .ToList())
            .ToList();
        if (notExistingSubs.Any())
            throw new ConflictException("These sub modules do not exist: " 
                                        + string.Join(", ", notExistingSubs.Select(x => x)));
        module.OrderedSubModules = orderedSubModules;
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<Guid> AddSubModule(Guid moduleId, SubModuleCreateDto model) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        var subModule = new SubModule {
            Name = model.Name,
            Module = module,
            SubModuleType = model.SubModuleType,
        };
        module.OrderedSubModules!.Add(subModule.Id);
        await _dbContext.AddAsync(subModule);
        await _dbContext.SaveChangesAsync();
        return subModule.Id;
    }

    public async Task EditSubModule(Guid subModuleId, SubModuleEditDto model) {
        var subModule = await _dbContext.SubModules
            .FirstOrDefaultAsync(m => m.Id == subModuleId && !m.ArchivedAt.HasValue);
        if (subModule == null) 
            throw new NotFoundException("Sub module not found");
        subModule.Name = model.Name;
        _dbContext.Update(subModule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveSubModule(Guid subModuleId) {
        var subModule = await _dbContext.SubModules
            .Include(s=>s.Module)
            .FirstOrDefaultAsync(m => m.Id == subModuleId);
        if (subModule == null) 
            throw new NotFoundException("Sub module not found");
        if (subModule.ArchivedAt.HasValue) 
            throw new ConflictException("Already archived");
        subModule.ArchivedAt = DateTime.UtcNow;
        subModule.Module.OrderedSubModules!.Remove(subModuleId);
        _dbContext.Update(subModule);
        await _dbContext.SaveChangesAsync();
    }
}