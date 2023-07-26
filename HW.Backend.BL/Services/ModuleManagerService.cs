using HW.Backend.BL.Extensions;
using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services; 

public class ModuleManagerService : IModuleManagerService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;

    public ModuleManagerService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task<PagedList<ModuleShortDto>> GetTeacherModules(PaginationParamsDto pagination, FilterModuleType? filter,
        ModuleFilterTeacherType? section, string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId) {
        /*var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);*/
        var modules = _dbContext.Modules
            .Where(m=>!m.ArchivedAt.HasValue)
            .ModuleTeacherFilter(filter, section, sortByNameFilter, userId)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        var shortModules = modules.Select(x =>new ModuleShortDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
            /*StartAt = typeof(Module) == x.GetType() ? null : _dbContext.StreamingModules.Find(x.Id)!.StartAt, // cringe
            ExpiredAt = typeof(Module) == x.GetType() ? null : _dbContext.StreamingModules.Find(x.Id)!.ExpiredAt,
            MaxStudents = typeof(Module) == x.GetType() ? null : _dbContext.StreamingModules.Find(x.Id)!.MaxStudents*/
        });
    return PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
     
    }

    public async Task CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User not teacher or not found");
        var creators = model.Creators!.Count == 0
            ? new List<Teacher>() { user } 
            : await _dbContext.Teachers.Where(t => model.Creators.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        
        var module = new Module {
            Name = model.Name,
            Description = model.Description ?? "",
            Price = model.Price ?? 0,
            ModuleVisibility = ModuleVisibilityType.OnlyMe,
            Creators = creators,
            Teachers = teachers
        };
        await _dbContext.AddAsync(module);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditSelfStudyModule(ModuleSelfStudyEditDto model, Guid moduleId) {
        var creators = model.Creators!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Creators.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        module.Name = model.Name;
        module.Description = model.Description;
        module.Price = model.Price;
        module.EditedAt = DateTime.UtcNow;
        module.ModuleVisibility = module.ModuleVisibility;
        if (creators.Count != 0) module.Creators = creators;
        if (teachers.Count != 0) module.Teachers = teachers;
         _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();    
    }

    public async Task CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) 
            throw new NotFoundException("User not teacher or not found");
        var creators = model.Creators!.Count == 0
            ? new List<Teacher>() { user } 
            : await _dbContext.Teachers.Where(t => model.Creators.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        
        var module = new StreamingModule() {
            Name = model.Name,
            Description = model.Description ?? "",
            Price = model.Price ?? 0,
            ModuleVisibility = ModuleVisibilityType.OnlyMe,
            Creators = creators,
            Teachers = teachers,
            StartAt = model.StartTime ?? DateTime.UtcNow.AddMonths(1),
            ExpiredAt = model.ExpirationTime,
            MaxStudents = model.MaxStudents ?? 0
        };
        await _dbContext.AddAsync(module);
        await _dbContext.SaveChangesAsync();    
    }

    public async Task EditStreamingModule(ModuleStreamingEditDto model, Guid moduleId) {
        var creators = model.Creators!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Creators.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        var module = await _dbContext.StreamingModules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        module.Name = model.Name;
        module.Description = model.Description;
        module.Price = model.Price;
        module.ModuleVisibility = module.ModuleVisibility;
        if (creators.Count != 0) module.Creators = creators;
        if (teachers.Count != 0) module.Teachers = teachers;
        module.StartAt = model.StartTime;
        module.ExpiredAt = model.ExpirationTime;
        module.MaxStudents = model.MaxStudents;
        module.EditedAt = DateTime.UtcNow;
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveModule(Guid moduleId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null) 
            throw new NotFoundException("Module not found");
        if (module == null) 
            throw new NotFoundException("Sub module not found");
        module.ArchivedAt = DateTime.UtcNow;
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddSubModule(Guid moduleId, SubModuleCreateDto model) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        var subModule = new SubModule {
            Name = model.Name,
            Module = module,
            SubModuleType = model.SubModuleType,
        };
        await _dbContext.AddAsync(subModule);
        await _dbContext.SaveChangesAsync();
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
            .FirstOrDefaultAsync(m => m.Id == subModuleId);
        if (subModule == null) 
            throw new NotFoundException("Sub module not found");
        if (subModule.ArchivedAt.HasValue) 
            throw new ConflictException("Already archived");
        subModule.ArchivedAt = DateTime.UtcNow;
        _dbContext.Update(subModule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateChapter(Guid subModuleId, ChapterCreateDto model) {
        var subModule = await _dbContext.SubModules
            .FirstOrDefaultAsync(m => m.Id == subModuleId && !m.ArchivedAt.HasValue);
        if (subModule == null) 
            throw new NotFoundException("Sub module not found");
        var chapter = new Chapter {
            Name = model.Name,
            Content = model.Content,
            SubModule = subModule,
            ChapterType = model.ChapterType
        };
        await _dbContext.AddAsync(chapter);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditChapter(Guid chapterId, ChapterEditDto model) {
        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(m => m.Id == chapterId && !m.ArchivedAt.HasValue);
        if (chapter == null) 
            throw new NotFoundException("Chapter not found");
        chapter.Name = model.Name;
        chapter.Content = model.Content;
        chapter.ChapterType = model.ChapterType;
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveChapter(Guid chapterId) {
        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        if (chapter == null) 
            throw new NotFoundException("Chapter not found");
        if (chapter.ArchivedAt.HasValue) 
            throw new ConflictException("Already archived");
        chapter.ArchivedAt = DateTime.UtcNow;
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();    
    }
}