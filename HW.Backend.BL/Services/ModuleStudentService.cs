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

public class ModuleStudentService : IModuleStudentService {
    private readonly ILogger<ModuleStudentService> _logger;
    private readonly IFileService _fileService;
    private readonly BackendDbContext _dbContext;

    public ModuleStudentService(BackendDbContext dbContext, ILogger<ModuleStudentService> logger, IFileService fileService) {
        _dbContext = dbContext;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<PagedList<ModuleShortDto>> GetAvailableModules(PaginationParamsDto pagination, FilterModuleType? filter, string? sortByNameFilter,
        SortModuleType? sortModuleType, Guid? userId) {
        var user = await _dbContext.Students
            .Include(u=>u.Modules)!
            .ThenInclude(m=>m.Module)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");
        
        var modules = _dbContext.Modules
            .Where(m => !m.ArchivedAt.HasValue && m.ModuleVisibility == ModuleVisibilityType.Everyone)
            .ModuleAvailableFilter(filter,sortByNameFilter)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        
        var shortModules = modules.Select( x =>new ModuleShortDto {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            AvatarId = x.AvatarId,
            TimeDuration = x.TimeDuration,
            StartDate = x is StreamingModule ?  ((StreamingModule)x).StartAt : null,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
        });
        var response = await PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
        foreach (var moduleShortDto in response.Items) {
            moduleShortDto.AvatarId = moduleShortDto.AvatarId == null
                ? moduleShortDto.AvatarId
                : await _fileService.GetAvatarLink(moduleShortDto.AvatarId);
            moduleShortDto.ModuleStatusType = user == null ? null :
                user.Modules!.All(m => m.Module.Id != moduleShortDto.Id) ? null :
                user.Modules!.FirstOrDefault(m => m.Module.Id == moduleShortDto.Id)!.ModuleStatus;
        }
        return response;
    }

    public async Task<PagedList<ModuleShortDto>> GetStudentModules(PaginationParamsDto pagination, FilterModuleType? filter, string? sortByNameFilter,
        ModuleStudentFilter? section, SortModuleType? sortModuleType, Guid userId) {
        var user = await _dbContext.Students
            .Include(u=>u.Modules)!
            .ThenInclude(m=>m.Module)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");
        
        var modules = _dbContext.Modules
            .Where(m => !m.ArchivedAt.HasValue)
            .ModuleStudentFilter(filter, section, sortByNameFilter, userId)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        
        var shortModules = modules.Select(x =>new ModuleShortDto {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            AvatarId = x.AvatarId,
            TimeDuration = x.TimeDuration,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
        });
        var response = await PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
        foreach (var moduleShortDto in response.Items) {
            moduleShortDto.AvatarId = moduleShortDto.AvatarId == null
                ? moduleShortDto.AvatarId
                : await _fileService.GetAvatarLink(moduleShortDto.AvatarId);
            moduleShortDto.ModuleStatusType = user == null
                ? null
                : user.Modules!.FirstOrDefault(m => m.Module.Id == moduleShortDto.Id)!.ModuleStatus;
            moduleShortDto.Progress = await CalculateProgressFloat(moduleShortDto.Id, userId);
        }
        return response;
    }

    public async Task<ModuleFullDto> GetModuleContent(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .Where(m=>!m.ArchivedAt.HasValue && m.ModuleVisibility == ModuleVisibilityType.Everyone)
            .Include(m=>m.SubModules)!
            .ThenInclude(s=>s.Chapters)!
            .ThenInclude(c=>c.LearnedList)!
            .ThenInclude(l=>l.LearnedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module == null)
            throw new NotFoundException("Module not found");

        
        
        var response = new ModuleFullDto {
            Id = module.Id,
            Progress = await CalculateProgressFloat(moduleId, userId),
            SubModules = module.SubModules != null? module.SubModules
                .Where(s=>!s.ArchivedAt.HasValue)
                .OrderBy(s=> module.OrderedSubModules!.IndexOf(s.Id))
                .Select(s=> new SubModuleFullDto {
                Id = s.Id,
                Name = s.Name,
                Chapters = s.Chapters != null ? s.Chapters
                    .Where(c=>!c.ArchivedAt.HasValue)
                    .OrderBy(c=> s.OrderedChapters!.IndexOf(c.Id))
                    .Select(c=> new ChapterShrotDto {
                    Id = c.Id,
                    Name = c.Name,
                    ChapterType = c.ChapterType,
                    IsLearned = c.LearnedList != null && c.LearnedList.Any(l=>l.LearnedBy.Id == userId)
                }).ToList() : new List<ChapterShrotDto>()
            }).ToList() : new List<SubModuleFullDto>()
        };
        foreach (var subModuleFullDto in response.SubModules) {
            foreach (var chapterShortDto in subModuleFullDto.Chapters) {
                if (chapterShortDto.IsLearned) continue;
                response.FirstUnlearnedChapter = chapterShortDto.Id;
                return response;
            }
        }
        return response;
    }

    public async Task<string> CalculateProgress(Guid moduleId, Guid userId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return "0/0";
        var chapters = await _dbContext.Chapters
            .Where(c => c.ChapterType == ChapterType.DefaultChapter && c.SubModule.Module.Id == moduleId)
            .AsNoTracking()
            .ToListAsync();
        var totalLearned = await _dbContext.Learned
            .Where(l => chapters.Contains(l.Chapter) && l.LearnedBy == user)
            .AsNoTracking()
            .CountAsync();
        return totalLearned + "/" + chapters.Count;
    }

    private async Task<float> CalculateProgressFloat(Guid moduleId, Guid userId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return 0;
        var chapters = await _dbContext.Chapters
            .Where(c => c.ChapterType == ChapterType.DefaultChapter && c.SubModule.Module.Id == moduleId)
            .AsNoTracking()
            .ToListAsync();
        var totalLearned = await _dbContext.Learned
            .Where(l => chapters.Contains(l.Chapter) && l.LearnedBy == user)
            .AsNoTracking()
            .CountAsync();
        return chapters.Count !=0 ? (float)Math.Round((float)totalLearned / chapters.Count, 2): 0;
    }

   

    public async Task<ModuleDetailsDto> GetModuleDetails(Guid moduleId, Guid? userId) {
        var module = await _dbContext.Modules
            .Include(m=>m.Editors)
            .Include(m=>m.UserModules)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Students
            .Include(u=>u.Modules)!
            .ThenInclude(m=>m.Module)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (module.Editors.IsNullOrEmpty() || (module.Editors!.All(c => c.Id != userId) &&
                                                module.ModuleVisibility == ModuleVisibilityType.OnlyCreators))
            throw new ForbiddenException("Module is private");
        
        var streamingModule = module as StreamingModule; 
        
        var response = new ModuleDetailsDto {
            Id = module.Id,
            Name = module.Name,
            Description = module.Description,
            Price = module.Price,
            Avatar = module.AvatarId == null 
                ? null 
                : new FileLinkDto {
                    FileId = _dbContext.Teachers.Any(t=>t.Id == userId) 
                        ? module.AvatarId 
                        : null,
                    Url = await _fileService.GetAvatarLink(module.AvatarId) 
                },
            Status = user?.Modules != null && user.Modules.Any(m => m.Module == module)
                ? user.Modules.FirstOrDefault(m => m.Module == module)!.ModuleStatus
                : ModuleStatusType.NotPurchased,
            Type = streamingModule == null ? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
            RequiredModules = module.RecommendedModules != null
                ? module.RecommendedModules.Select(m => new RequiredModulesDto {
                    Id = m.Id,
                    Avatar = new FileLinkDto{
                        FileId = !module.Editors.IsNullOrEmpty() && module.Editors!.Any(c=>c.Id == userId) 
                            ? m.AvatarId 
                            : null,
                        Url = m.AvatarId
                        },
                    Name = m.Name,
                    Status = user?.Modules != null && user.Modules.Any(sm => sm.Module == m)
                        ? user.Modules.FirstOrDefault(sm => sm.Module == m)!.ModuleStatus
                        : ModuleStatusType.NotPurchased
                }).ToList()
                : new List<RequiredModulesDto>(),
            VisibilityType = !module.Editors.IsNullOrEmpty() && module.Editors!
                .Any(c=>c.Id == userId) ? module.ModuleVisibility : null,
            AmountOfStudents = module.UserModules.IsNullOrEmpty() 
                ? 0 
                : module.UserModules!.Count(um => um.ModuleStatus 
                is ModuleStatusType.Purchased or ModuleStatusType.InProcess),
            Author = module.Editors.IsNullOrEmpty() 
                ? Guid.Empty 
                : module.Editors!.FirstOrDefault()!.Id,
            Editors = !module.Editors.IsNullOrEmpty() && module.Editors!.Any(c=>c.Id == userId)
                ? module.Editors!.Select(e=>e.Id).ToList() : new List<Guid>(),
            Teachers = !module.Editors.IsNullOrEmpty() && module.Editors!.Any(c=>c.Id == userId)
                ? module.Teachers!.Select(e=>e.Id).ToList() : new List<Guid>(),
            StartDate = streamingModule?.StartAt,
            StopRegistrationDate = streamingModule?.StopRegisterAt,
            StartRegistrationDate = streamingModule?.StartRegisterAt,
            ExpirationDate = streamingModule?.ExpiredAt,
            MaxStudents = streamingModule?.MaxStudents,
            TimeDuration = module.TimeDuration
        };
        
        foreach (var responseRequiredModule in response.RequiredModules) {
            if (responseRequiredModule.Avatar != null)
                responseRequiredModule.Avatar.Url = responseRequiredModule.Avatar.Url == null
                    ? null
                    : await _fileService.GetAvatarLink(responseRequiredModule.Avatar.Url);
        }
        return response;
    }

    public async Task<ModuleDetailsDto> SendCommentToModule(ModuleCommentDto model, Guid moduleId, Guid userId) {
        throw new NotImplementedException();
    }

    public async Task BuyModule(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId 
                                      && !m.ArchivedAt.HasValue
                                      && m.ModuleVisibility == ModuleVisibilityType.Everyone);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.UserBackends
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) {
            user = new UserBackend {
                Id = userId
            };
            await _dbContext.UserBackends.AddAsync(user);
        }

        if (user.Student == null) {
            user.Student = new Student {
                Id = userId, UserBackend = user
            };
            await _dbContext.Students.AddAsync(user.Student);
        }

        var studentModule = await _dbContext.UserModules
                .FirstOrDefaultAsync(um => um.Student == user.Student && um.Module == module);
        if (studentModule == null) {
            studentModule = new StudentModule
            {
                Student = user.Student,
                Module = module,
                ModuleStatus = ModuleStatusType.Purchased
            };
            await _dbContext.UserModules.AddAsync(studentModule);
        }
        else {
            if (studentModule.ModuleStatus != ModuleStatusType.InCart)
                throw new ConflictException("User already has this module");
            studentModule.ModuleStatus = ModuleStatusType.Purchased;
            _dbContext.Update(studentModule);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task StartModule(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId 
                                      && !m.ArchivedAt.HasValue
                                      && m.ModuleVisibility == ModuleVisibilityType.Everyone);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.UserBackends
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");
        var studentModule = await _dbContext.UserModules
            .FirstOrDefaultAsync(um => um.Student == user.Student && um.Module == module);
        if (studentModule is not { ModuleStatus: ModuleStatusType.Purchased })
            throw new ForbiddenException("User don't have this module");
        studentModule.ModuleStatus = ModuleStatusType.InProcess;
        _dbContext.Update(studentModule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddModuleToBasket(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId 
                                      && !m.ArchivedAt.HasValue
                                      && m.ModuleVisibility == ModuleVisibilityType.Everyone);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.UserBackends
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) {
            user = new UserBackend {
                Id = userId
            };
           await _dbContext.UserBackends.AddAsync(user);
        }

        if (user.Student == null) {
            user.Student = new Student {
                Id = userId, UserBackend = user
            };
            await _dbContext.Students.AddAsync(user.Student);
        }

        if (await _dbContext.UserModules
                .AnyAsync(um => um.Student == user.Student && um.Module == module)) {
            throw new ConflictException("User already has this module");
        }

        var studentModule = new StudentModule
        {
            Student = user.Student,
            Module = module,
            ModuleStatus = ModuleStatusType.InCart
        };

       await _dbContext.UserModules.AddAsync(studentModule);
       await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteModuleFromBasket(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Students
            .Include(u => u.Modules)!
            .ThenInclude(m=>m.Module)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");
        if (user.Modules!.All(m => m.Module != module))
            throw new ConflictException("User not have this module in basket");
        var studentModule = user.Modules!
            .FirstOrDefault(m => m.Module == module);
        if (studentModule!.ModuleStatus != ModuleStatusType.InCart)
            throw new ConflictException("User probably already bought this course");
        _dbContext.Remove(studentModule);
        await _dbContext.SaveChangesAsync();
    }
}