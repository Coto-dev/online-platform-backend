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
        ModuleFilterTeacherType? section, string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId) {
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
            /*.Include(m=>m.SubModules)!
            .ThenInclude(s=>s.Chapters)
            .AsNoTracking()*/
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        var subModules = await _dbContext.SubModules
            .Where(s => s.Module.Id == moduleId)
            .ToListAsync();
        (subModules[4], subModules[3]) = (subModules[3], subModules[4]);
        _dbContext.Update(subModules[3]);
        _dbContext.Update(subModules[4]);
        //_dbContext.UpdateRange(subModules);
        await _dbContext.SaveChangesAsync();
        if (module == null)
            throw new NotFoundException("Module not found");
        return new ModuleFullTeacherDto {
            Id = module.Id,
            SubModules = subModules.Select(s=> new SubModuleFullDto {
                Id = s.Id,
                Name = s.Name,
                Chapters = s.Chapters != null ? s.Chapters.Select(c=> new ChapterShrotDto {
                    Id = c.Id,
                    Name = c.Name,
                    ChapterType = c.ChapterType
                }).ToList() : new List<ChapterShrotDto>()
            }).ToList()
        };
    }
    
    public async Task<ChapterFullTeacherDto> GetChapterContent(Guid chapterId, Guid userId) {
        var chapter = await _dbContext.Chapters
            .Include(c=>c.ChapterTests)
            .Include(c=>c.ChapterComments)!
            .ThenInclude(com=>com.User)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        var user = await _dbContext.Students
            .Include(u=>u.LearnedChapters)
            .FirstOrDefaultAsync(u => u.Id == userId);
        var response = new ChapterFullTeacherDto {
            Id = chapter!.Id,
            Name = chapter.Name,
            Content = chapter.Content ?? "",
            FileIds = chapter.Files == null
                ? new List<FileLinkDto>()
                : chapter.Files.Select( f => new FileLinkDto {
                    FileId = f,
                    Url = null //TODO 
                }).ToList(),
            Comments = chapter.ChapterComments == null
                ? new List<ChapterCommentDto>()
                : chapter.ChapterComments.Select(com => new ChapterCommentDto {
                    Id = com.Id,
                    UserId = com.User.Id,
                    IsTeacherComment = com.IsTeacherComment,
                    Message = com.Comment
                }).ToList(),
            ChapterType = chapter.ChapterType,
            Tests = chapter.ChapterTests == null
                ? new List<TestTeacherDto>()
                : chapter.ChapterTests.Select(t => new TestTeacherDto {
                    Id = t.Id,
                    Question = t.Question,
                    FileIds = t.Files == null
                        ? new List<FileLinkDto>()
                        : t.Files.Select( f => new FileLinkDto {
                            FileId = f,
                            Url = null //TODO 
                        }).ToList(),
                    PossibleSimpleAnswers = t is SimpleAnswerTest simpleAnswerTest ? simpleAnswerTest.PossibleAnswers
                        .Select(uat=> new SimpleAnswerDto {
                            AnswerContent = uat.AnswerContent,
                            isRight = uat.IsRight
                        }).ToList():new List<SimpleAnswerDto>(),
                    PossibleCorrectSequenceAnswers = t is CorrectSequenceTest correctSequenceTest ? correctSequenceTest.PossibleAnswers
                        .Select(uat=> new CorrectSequenceAnswerDto {
                            AnswerContent = uat.AnswerContent,
                            RightOrder = uat.RightOrder
                        }).ToList():new List<CorrectSequenceAnswerDto>(),
                    Type = t.TestType,
                }).ToList()
        };
        foreach (var responseFileId in response.FileIds) {
            responseFileId.Url = (await _fileService.GetFileLink(responseFileId.FileId!) ?? null)!;
        }
        foreach (var fileLinkDto in response.Tests.SelectMany(testTeacherDto => testTeacherDto.FileIds!)) {
            fileLinkDto.Url = (await _fileService.GetFileLink(fileLinkDto.FileId!) ?? null)!;
        }
        return response;
    }
    public async Task CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId) {
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
    }

    public async Task EditSelfStudyModule(ModuleSelfStudyEditDto model, Guid moduleId) {
        var editors = model.Editors!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Editors.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module is null or StreamingModule) 
            throw new NotFoundException("Module not found");
        module.TimeDuration = model.TimeDuration;
        module.Name = model.Name;
        module.Description = model.Description;
        module.Price = model.Price;
        module.EditedAt = DateTime.UtcNow;
        module.AvatarId = model.AvatarId;
        module.ModuleVisibility = model.VisibilityType;
        if (editors.Count != 0) module.Editors = editors;
        if (teachers.Count != 0) module.Teachers = teachers;
        if (!editors.Contains(module.Author))
            editors.Add(module.Author); 
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();    
    }

    public async Task CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId) {
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
            ? new List<Teacher>()
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
            ExpiredAt = model.ExpirationTime,
            MaxStudents = model.MaxStudents ?? 0
        };
        await _dbContext.AddAsync(module);
        await _dbContext.SaveChangesAsync();    
    }

    public async Task EditStreamingModule(ModuleStreamingEditDto model, Guid moduleId) {
        var editors = model.Editors!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Editors.Contains(t.Id)).ToListAsync();
        var teachers = model.Teachers!.Count == 0
            ? new List<Teacher>()
            : await _dbContext.Teachers.Where(t => model.Teachers.Contains(t.Id)).ToListAsync();
        var module = await _dbContext.StreamingModules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null) 
            throw new NotFoundException("Module not found");
        module.TimeDuration = model.TimeDuration;
        module.Name = model.Name;
        module.Description = model.Description;
        module.Price = model.Price;
        module.AvatarId = model.AvatarId;
        module.ModuleVisibility = model.VisibilityType;
        if (editors.Count != 0) module.Editors = editors;
        if (teachers.Count != 0) module.Teachers = teachers;
        module.StartAt = model.StartTime;
        module.ExpiredAt = model.ExpirationTime;
        module.MaxStudents = model.MaxStudents;
        module.EditedAt = DateTime.UtcNow;
        if (!editors.Contains(module.Author))
            editors.Add(module.Author); 
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
        module.OrderedSubModules!.Add(subModule.Id);
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

    public async Task CreateChapter(Guid subModuleId, ChapterCreateDto model) {
        var subModule = await _dbContext.SubModules
            .FirstOrDefaultAsync(m => m.Id == subModuleId && !m.ArchivedAt.HasValue);
        if (subModule == null) 
            throw new NotFoundException("Sub module not found");
        var chapter = new Chapter {
            Name = model.Name,
            Content = model.Content,
            SubModule = subModule,
            ChapterType = model.ChapterType,
            Files = model.FileIds
        };
        subModule.OrderedChapters!.Add(chapter.Id);
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
        chapter.Files = model.FileIds;
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveChapter(Guid chapterId) {
        var chapter = await _dbContext.Chapters
            .Include(c=>c.SubModule)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        if (chapter == null) 
            throw new NotFoundException("Chapter not found");
        if (chapter.ArchivedAt.HasValue) 
            throw new ConflictException("Already archived");
        chapter.ArchivedAt = DateTime.UtcNow;
        chapter.SubModule.OrderedChapters!.Remove(chapterId);
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();    
    }
}