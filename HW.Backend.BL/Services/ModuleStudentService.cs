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
            .Where(m => !m.ArchivedAt.HasValue)
            .ModuleAvailableFilter(filter,sortByNameFilter)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        
        var shortModules = modules.Select( x =>new ModuleShortDto {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            AvatarId = x.AvatarId,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
        });
        var response = await PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
        foreach (var moduleShortDto in response.Items) {
            moduleShortDto.AvatarId = moduleShortDto.AvatarId == null
                ? moduleShortDto.AvatarId
                : await _fileService.GetAvatarLink(moduleShortDto.AvatarId);
            moduleShortDto.ModuleStatusType = user == null ? null :
                user.Modules!.All(m => m.Module.Id != moduleShortDto.Id) ? null :
                user.Modules!.FirstOrDefault(m => m.Module.Id == moduleShortDto.Id)!.ModuleStatus ==
                ModuleStatusType.InCart ? ModuleStatusType.InCart :
                ModuleStatusType.Purchased;
        }
        return response;
    }

    public async Task<PagedList<ModuleShortDto>> GetStudentModules(PaginationParamsDto pagination, FilterModuleType? filter, string? sortByNameFilter,
        ModuleFilterStudentType? section, SortModuleType? sortModuleType, Guid userId) {
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

    public async Task<ModuleFullDto> GetModuleContent(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .Include(m=>m.SubModules)!
            .ThenInclude(s=>s.Chapters)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        return new ModuleFullDto {
            Id = module.Id,
            Progress = await CalculateProgress(moduleId, userId),
            SubModules = module.SubModules != null? module.SubModules
                .OrderBy(s=>s.CreatedAt)
                .Select(s=> new SubModuleFullDto {
                Id = s.Id,
                Name = s.Name,
                Chapters = s.Chapters != null ? s.Chapters
                    .OrderBy(c=>c.CreatedAt)
                    .Select(c=> new ChapterShrotDto {
                    Id = c.Id,
                    Name = c.Name,
                    ChapterType = c.ChapterType
                }).ToList() : new List<ChapterShrotDto>()
            }).ToList() : new List<SubModuleFullDto>()
        };
        
    }

    public async Task<string> CalculateProgress(Guid moduleId, Guid userId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User not found");
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

    public async Task<ChapterFullDto> GetChapterContent(Guid chapterId, Guid userId) {
        var chapter = await _dbContext.Chapters
            .Include(c=>c.ChapterTests)
            .Include(c=>c.ChapterComments)!
            .ThenInclude(com=>com.User)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        var user = await _dbContext.Students
            .Include(u=>u.LearnedChapters)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return new ChapterFullDto {
            Id = chapter!.Id,
            Name = chapter.Name,
            Content = chapter.Content ?? "",
            FileUrls = chapter.Files.IsNullOrEmpty()
                ? new List<string>()
                : chapter.Files!.Select(async f=> await _fileService.GetFileLink(f)).Select(task=>task.Result).ToList()!, 
            Comments = chapter.ChapterComments == null  
                ? new List<ChapterCommentDto>()
                : chapter.ChapterComments.Select(com => new ChapterCommentDto {
                    Id = com.Id,
                    UserId = com.User.Id,
                    IsTeacherComment = com.IsTeacherComment,
                    Message = com.Comment
                }).ToList(),
            IsLearned = user!.LearnedChapters != null && user.LearnedChapters.Contains(chapter),
            ChapterType = chapter.ChapterType,
            Tests = chapter.ChapterTests == null
                ? new List<TestDto>()
                : chapter.ChapterTests.Select(t => new TestDto {
                    Id = t.Id,
                    Question = t.Question,
                    FileIds = t.Files.IsNullOrEmpty()
                        ? new List<string>()
                        : t.Files!.Select(async f=> await _fileService.GetFileLink(f)).Select(task=>task.Result).ToList()!, 
                    PossibleAnswers = t switch {
                        SimpleAnswerTest simpleTest => simpleTest.PossibleAnswers
                            .Select(pa => new PossibleAnswerDto {
                                Id = pa.Id,
                                AnswerContent = pa.AnswerContent
                            }).ToList(),
                        CorrectSequenceTest correctSequenceTest => correctSequenceTest.PossibleAnswers
                            .Select(pa => new PossibleAnswerDto {
                                Id = pa.Id,
                                AnswerContent = pa.AnswerContent
                            }).ToList(),
                        _ => new List<PossibleAnswerDto>()
                    },
                    UserAnswer = 
                        _dbContext.UserAnswerTests.Any(uat=>uat.Student == user && uat.Test == t) ?
                            t.TestType is TestType.ExtraAnswer 
                        or TestType.MultipleAnswer 
                        or TestType.SingleAnswer 
                        or TestType.MultipleExtraAnswer ? new UserAnswerFullDto {
                        UserAnswerSimples = _dbContext.UserAnswers.OfType<SimpleUserAnswer>()
                            .Where(u=>u.UserAnswerTest.Test == t && u.UserAnswerTest.Student == user)
                            .Select(s=> new UserAnswerSimpleDto {
                                Id = s.SimpleAnswer.Id
                            }).ToList(),
                        IsAnswered =  _dbContext.UserAnswerTests
                            .Where(uat=>uat.Student == user && uat.Test == t)
                            .MaxBy(uat=>uat.NumberOfAttempt)!.AnsweredAt.HasValue 
                    } : t.TestType is TestType.CorrectSequenceAnswer ? new UserAnswerFullDto {
                                UserAnswerCorrectSequences = _dbContext.UserAnswers.OfType<CorrectSequenceUserAnswer>()
                                .Where(u=>u.UserAnswerTest.Test == t && u.UserAnswerTest.Student == user)
                                .Select(s=> new UserAnswerCorrectSequenceDto() {
                                Id = s.CorrectSequenceAnswer.Id,
                                Order = s.Order
                                }).ToList(),
                                IsAnswered = _dbContext.UserAnswerTests
                                    .Where(uat=>uat.Student == user && uat.Test == t)
                                    .MaxBy(uat=>uat.NumberOfAttempt)!.AnsweredAt.HasValue 
                            } : t.TestType is TestType.DetailedAnswer ? new UserAnswerFullDto {
                                DetailedAnswer = _dbContext.UserAnswers.OfType<DetailedAnswer>()
                                    .FirstOrDefault(u=>u.UserAnswerTest.Test == t && u.UserAnswerTest.Student == user)!
                                    .AnswerContent,
                                IsAnswered = _dbContext.UserAnswerTests
                                    .Where(uat=>uat.Student == user && uat.Test == t)
                                    .MaxBy(uat=>uat.NumberOfAttempt)!.AnsweredAt.HasValue
                            } : null : null,
                    Type = t.TestType,
                }).ToList()
        };
    }

    public async Task<ModuleDetailsDto> GetModuleDetails(Guid moduleId, Guid? userId) {
        var module = await _dbContext.Modules
            .Include(m=>m.Creators)
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        var user = await _dbContext.Students
            .Include(u=>u.Modules)!
            .ThenInclude(m=>m.Module)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        var streamingModule = module as StreamingModule; 
        
        var response = new ModuleDetailsDto {
            Id = module.Id,
            Name = module.Name,
            Description = module.Description,
            Price = module.Price,
            Avatar = module.AvatarId == null 
                ? null 
                : new FileLinkDto {
                    FileId = _dbContext.Teachers.Any(t=>t.Id == userId) ? module.AvatarId : null,
                    Url = await _fileService.GetAvatarLink(module.AvatarId) 
                },
            Status = user?.Modules != null && user.Modules.Any(m => m.Module == module)
                ? user.Modules.FirstOrDefault(m => m.Module == module)!.ModuleStatus
                : ModuleStatusType.NotPurchased,
            Type = ModuleType.StreamingModule,
            RequiredModules = module.RecommendedModules != null
                ? module.RecommendedModules.Select(m => new RequiredModulesDto {
                    Id = m.Id,
                    Avatar = new FileLinkDto{
                        FileId = !module.Creators.IsNullOrEmpty() ? module.Creators!.Any(c=>c.Id == userId) ? m.AvatarId : null : null,
                        Url = m.AvatarId
                        },
                    Name = m.Name
                }).ToList()
                : new List<RequiredModulesDto>(),
            StartDate = streamingModule?.StartAt ,
            ExpirationDate = streamingModule?.ExpiredAt,
            MaxStudents = streamingModule?.MaxStudents,
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
            .FirstOrDefaultAsync(m => m.Id == moduleId && m.ModuleVisibility == ModuleVisibilityType.Everyone);
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

    public async Task AddModuleToBasket(Guid moduleId, Guid userId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && m.ModuleVisibility == ModuleVisibilityType.Everyone);
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