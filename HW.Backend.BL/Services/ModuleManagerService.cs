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
using Microsoft.IdentityModel.Tokens;

namespace HW.Backend.BL.Services; 

public class ModuleManagerService : IModuleManagerService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;
    private readonly IFileService _fileService;

    public ModuleManagerService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext, IFileService fileService) {
        _logger = logger;
        _dbContext = dbContext;
        _fileService = fileService;
    }


    public async Task<PagedList<ModuleShortDto>> GetTeacherModules(PaginationParamsDto pagination, FilterModuleType? filter,
        ModuleTeacherFilter? section, string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");
        
        var modules = _dbContext.Modules
            .Where(m => !m.ArchivedAt.HasValue)
            .ModuleTeacherFilter(filter, section, sortByNameFilter, userId)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        
        var shortModules = modules.Select(x =>new ModuleShortDto {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            AvatarId = x.AvatarId,
            TimeDuration = x.TimeDuration,
            UserType = x.Author == user 
                ? UserType.Author
                : x.Editors!.Contains(user!)
                ? UserType.Editor 
                : null,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
        });
        var response = await PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
        foreach (var moduleShortDto in response.Items) {
            moduleShortDto.AvatarId = moduleShortDto.AvatarId == null
                ? moduleShortDto.AvatarId
                : await _fileService.GetAvatarLink(moduleShortDto.AvatarId);
        }
        return response;
    }
    public async Task<ModuleFullTeacherDto> GetModuleContent(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .Include(m=>m.SubModules)!
            .ThenInclude(s=>s.Chapters)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        return new ModuleFullTeacherDto {
            Id = module.Id,
            SubModules = module.SubModules!
                .Where(s=>!s.ArchivedAt.HasValue)
                .OrderBy(x=> module.OrderedSubModules!.IndexOf(x.Id))
                .Select(s=> new SubModuleFullDto {
                Id = s.Id,
                Name = s.Name,
                Chapters = s.Chapters != null ? s.Chapters
                    .Where(с=>!с.ArchivedAt.HasValue)
                    .OrderBy(c=> s.OrderedChapters!.IndexOf(c.Id))
                    .Select(c=> new ChapterShrotDto {
                    Id = c.Id,
                    Name = c.Name,
                    ChapterType = c.ChapterType
                }).ToList() : new List<ChapterShrotDto>()
            }).ToList()
        };
    }

    public async Task EditModuleSortStructure(SortStructureDto structureDto, Guid moduleId) {
        var module = await _dbContext.Modules
            .Include(m => m.SubModules)!
            .ThenInclude(s => s.Chapters)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var missingChapters = new List<Guid>();
        var notExistingChaptersModule = new List<Guid>();
        foreach (var subModuleSort in structureDto.SubModules) {
            var subModule = module.SubModules!
                .FirstOrDefault(s => s.Id == subModuleSort.Id);
            if (subModule == null)
                throw new NotFoundException("Sub module not found"); 
            
            var duplicates = subModuleSort.ChapterIds
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
            if (duplicates.Count > 0)
                throw new BadRequestException("There are chapter duplicates: " + string.Join(", ", duplicates.Select(x => x)));
            
            missingChapters.AddRange(subModule.Chapters!
                .Where(c=>!c.ArchivedAt.HasValue)
                .Select(c=>c.Id)
                .Except(subModuleSort.ChapterIds)
                .ToList());
            
            var notExistingChapters = subModuleSort.ChapterIds.Except(subModule.Chapters!
                    .Where(c=>!c.ArchivedAt.HasValue)
                    .Select(c=>c.Id)
                    .ToList())
                .ToList();
            notExistingChaptersModule.AddRange(notExistingChapters);
            var chapters = module.SubModules!
                .SelectMany(s => s.Chapters!
                    .Where(c=> notExistingChapters.Contains(c.Id))
                    .ToList())
                .ToList();
           var notFoundChapters = notExistingChapters
               .Where(nec => chapters.All(c => c.Id != nec))
               .ToList();
           if (notFoundChapters.Any()) 
               throw new ConflictException("These chapters not found in this module: " 
                                           + string.Join(", ", notFoundChapters.Select(x => x)));
           
            chapters.ForEach(c=>c.SubModule = subModule);
            subModule.OrderedChapters = subModuleSort.ChapterIds;
        }
        foreach (var notExChapter in notExistingChaptersModule) {
            missingChapters.Remove(notExChapter);
        }
        if (missingChapters.Any()) 
            throw new ConflictException("These chapters are missing: " 
                                        + string.Join(", ", missingChapters.Select(x => x)));
        var subsDuplicates = structureDto.SubModules
            .Select(s=>s.Id)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (subsDuplicates.Count > 0)
            throw new BadRequestException("There are sub module duplicates: " + string.Join(", ", subsDuplicates.Select(x => x)));
        
        var missingSubs = module.SubModules!
            .Where(s=>!s.ArchivedAt.HasValue)
            .Select(o=>o.Id)
            .Except(structureDto.SubModules.Select(s=>s.Id))
            .ToList();
        if (missingSubs.Any())
            throw new ConflictException("These sub modules are missing: " 
                                        + string.Join(", ", missingSubs.Select(x => x)));
        var notExistingSubs = structureDto.SubModules.Select(s=>s.Id)
            .Except(module.SubModules!
                .Where(s=>!s.ArchivedAt.HasValue)
                .Select(o=>o.Id)
                .ToList())
            .ToList();
        if (notExistingSubs.Any())
            throw new ConflictException("These sub modules do not exist: " 
                                        + string.Join(", ", notExistingSubs.Select(x => x)));
        module.OrderedSubModules = structureDto.SubModules
            .Select(s => s.Id)
            .ToList();
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();

    }

    public async Task<Guid> CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId) {
        var user = await _dbContext.UserBackends
            .Include(u=>u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            var newUser = new UserBackend() {
                Id = userId
            };
            user = newUser;
            await _dbContext.AddAsync(user);
        }
        user.Teacher ??= new Teacher {
            Id = user.Id,
            UserBackend = user
        };
        var editors = model.Editors!.Count == 0
            ? new List<Teacher>() { user.Teacher } 
            : await _dbContext.Teachers.Where(t => model.Editors.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>() { user.Teacher } 
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        
        var module = new Module {
            TimeDuration = model.TimeDuration,
            Author = user.Teacher,
            Name = model.Name,
            Description = model.Description ?? "",
            Price = model.Price ?? 0,
            ModuleVisibility = ModuleVisibilityType.OnlyCreators,
            AvatarId = model.AvatarId,
            Editors = editors.IsNullOrEmpty() ? new List<Teacher>() { user.Teacher } : editors,
            Teachers = teachers.IsNullOrEmpty() ? new List<Teacher>() { user.Teacher } : teachers
        };
        await _dbContext.AddAsync(module);
        await _dbContext.SaveChangesAsync();
        return module.Id;
    }

    public async Task EditSelfStudyModule(ModuleSelfStudyEditDto model, Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module is null or StreamingModule) 
            throw new NotFoundException("Module not found");
        module.TimeDuration = model.TimeDuration;
        module.Name = model.Name;
        module.Description = model.Description;
        module.Price = model.Price;
        module.EditedAt = DateTime.UtcNow;
        if (module.AvatarId != null && module.AvatarId != model.AvatarId)
            await _fileService.RemoveFiles(new List<string>(){module.AvatarId});
        module.AvatarId = model.AvatarId;

        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();    
    }

    public async Task EditVisibilityModule(ModuleVisibilityType visibilityType, Guid moduleId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        module.ModuleVisibility = visibilityType;
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();  
    }

    public async Task<Guid> CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId) {
        var user = await _dbContext.UserBackends
            .Include(u=>u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) {
            var newUser = new UserBackend() {
                Id = userId
            };
            user = newUser;
            await _dbContext.AddAsync(user);
        }
        user.Teacher ??= new Teacher {
            Id = user.Id,
            UserBackend = user
        };
        
        var editors = model.Editors!.Count == 0
            ? new List<Teacher>() { user.Teacher } 
            : await _dbContext.Teachers.Where(t => model.Editors.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>() {user.Teacher}
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        
        var module = new StreamingModule() {
            TimeDuration = model.TimeDuration,
            Author = user.Teacher,
            Name = model.Name,
            Description = model.Description ?? "",
            Price = model.Price ?? 0,
            ModuleVisibility = ModuleVisibilityType.OnlyCreators,
            AvatarId = model.AvatarId,
            Editors = editors.IsNullOrEmpty() ? new List<Teacher>() { user.Teacher } : editors,
            Teachers = teachers.IsNullOrEmpty() ? new List<Teacher>() { user.Teacher } : teachers,
            StartAt = model.StartTime ?? DateTime.UtcNow.AddMonths(1),
            StartRegisterAt = model.StartRegistrationDate,
            StopRegisterAt = model.StopRegistrationDate,
            ExpiredAt = model.ExpirationTime,
            MaxStudents = model.MaxStudents ?? 0
        };
        await _dbContext.AddAsync(module);
        await _dbContext.SaveChangesAsync();  
        return module.Id;
    }

    public async Task EditStreamingModule(ModuleStreamingEditDto model, Guid moduleId, Guid userId) {
        var module = await _dbContext.StreamingModules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        module.TimeDuration = model.TimeDuration;
        module.Name = model.Name;
        module.Description = model.Description;
        module.Price = model.Price;
        if (module.AvatarId != null && module.AvatarId != model.AvatarId)
            await _fileService.RemoveFiles(new List<string>(){module.AvatarId});
        module.AvatarId = model.AvatarId;
        module.StartAt = model.StartTime;
        module.ExpiredAt = model.ExpirationTime;
        module.MaxStudents = model.MaxStudents;
        module.StartRegisterAt = model.StartRegistrationDate;
        module.StopRegisterAt = model.StopRegistrationDate;
        module.EditedAt = DateTime.UtcNow;

        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddEditorToModule(Guid userId, Guid moduleId) {
        var module = await _dbContext.Modules
            .Include(m=>m.Editors)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");
        
        if (module.Editors!.Contains(user))
            throw new ConflictException("User already editor");
        module.Editors.Add(user);
       await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveEditorFromModule(Guid userId, Guid moduleId) {
        var module = await _dbContext.Modules
            .Include(m=>m.Editors)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!module.Editors!.Contains(user))
            throw new ConflictException("User is not editor");
        module.Editors.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddTeacherToModule(Guid userId, Guid moduleId) {
        var module = await _dbContext.Modules
            .Include(m=>m.Teachers)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (module.Teachers!.Contains(user))
            throw new ConflictException("User already teacher");
        module.Teachers.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveTeacherFromModule(Guid userId, Guid moduleId) {
        var module = await _dbContext.Modules
            .Include(m=>m.Teachers)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!module.Teachers!.Contains(user))
            throw new ConflictException("User is not teacher");
        module.Teachers.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveModule(Guid moduleId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        module.ArchivedAt = DateTime.UtcNow;
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();
        await _dbContext.SaveChangesAsync();
    }
    
}