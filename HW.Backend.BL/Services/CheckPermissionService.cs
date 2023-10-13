using HW.Backend.DAL.Data;
using HW.Common.Enums;
using HW.Backend.DAL.Data.Entities;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services; 

public class CheckPermissionService: ICheckPermissionService {
    private readonly ILogger<CheckPermissionService> _logger;
    private readonly BackendDbContext _dbContext;

    public CheckPermissionService(ILogger<CheckPermissionService> logger, BackendDbContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task CheckAuthorModulePermission(Guid authorId, Guid moduleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == authorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (module.Author != user)
            throw new ForbiddenException("User do not have permission");
    }

    public async Task CheckCreatorModulePermission(Guid creatorId, Guid moduleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var module = await _dbContext.Modules
            .Include(m=>m.Editors)
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (module.Editors != null && !module.Editors.Contains(user))
            throw new ForbiddenException("User do not have permission");

    }

    public async Task CheckTeacherModulePermission(Guid teacherId, Guid moduleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == teacherId);
        if (user == null)
            throw new NotFoundException("User not found");
        var module = await _dbContext.Modules
            .Include(m=>m.Teachers)
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (module.Teachers == null || !module.Teachers.Contains(user))
            throw new ForbiddenException("User do not have permission");
        
    }

    public async Task CheckStudentModulePermission(Guid studentId, Guid moduleId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == studentId);
        if (user == null)
            throw new NotFoundException("User not found");
        var module = await _dbContext.Modules
            .Include(m=>m.UserModules)! // <------------------------------- warning
            .ThenInclude(u=>u.Student)            
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (!module.UserModules!.Any(u => u.Student == user && u.ModuleStatus!= ModuleStatusType.InCart))
            throw new ForbiddenException("User do not have permission");
    }

    public async Task CheckCreatorSubModulePermission(Guid creatorId, Guid subModuleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var subModule = await _dbContext.SubModules
            .Include(s=>s.Module)
            .ThenInclude(m=>m.Editors)
            .FirstOrDefaultAsync(u => u.Id == subModuleId);
        if (subModule == null)
            throw new NotFoundException(" Sub module not found");    
        if (subModule.Module.Editors != null && !subModule.Module.Editors.Contains(user))
            throw new ForbiddenException("User do not have permission");

    }

    public async Task CheckTeacherSubModulePermission(Guid teacherId, Guid subModuleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == teacherId);
        if (user == null)
            throw new NotFoundException("User not found");
        var subModule = await _dbContext.SubModules
            .Include(s=>s.Module)
            .ThenInclude(m=>m.Teachers)
            .FirstOrDefaultAsync(u => u.Id == subModuleId);
        if (subModule == null)
            throw new NotFoundException(" Sub module not found");    
        if (subModule.Module.Teachers == null || !subModule.Module.Teachers.Contains(user))
            throw new ForbiddenException("User do not have permission");    
    }

    public async Task CheckStudentSubModulePermission(Guid studentId, Guid subModuleId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == studentId);
        if (user == null)
            throw new NotFoundException("User not found");
        var subModule = await _dbContext.SubModules
            .Include(s=>s.Module)
            .ThenInclude(m=>m.UserModules)!                                  //<------------------------
            .ThenInclude(u=>u.Student)
            .FirstOrDefaultAsync(u => u.Id == subModuleId);
        if (subModule == null)
            throw new NotFoundException("Sub module not found");    
        if (!subModule.Module.UserModules!.Any(u => u.Student == user && u.ModuleStatus != ModuleStatusType.InCart))
            throw new ForbiddenException("User do not have permission");
        
    }

    public async Task CheckCreatorChapterPermission(Guid creatorId, Guid chapterId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var chapter = await _dbContext.Chapters
            .Include(c=>c.SubModule)
            .ThenInclude(s=>s.Module)
            .ThenInclude(m=>m.Editors)
            .FirstOrDefaultAsync(u => u.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");    
        if (chapter.SubModule.Module.Editors != null && !chapter.SubModule.Module.Editors.Contains(user))
            throw new ForbiddenException("User do not have permission");    
    }

    public async Task CheckTeacherChapterPermission(Guid teacherId, Guid chapterId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == teacherId);
        if (user == null)
            throw new NotFoundException("User not found");
        var chapter = await _dbContext.Chapters
            .Include(c=>c.SubModule)
            .ThenInclude(s=>s.Module)
            .ThenInclude(m=>m.Teachers)
            .FirstOrDefaultAsync(u => u.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");    
        if (chapter.SubModule.Module.Teachers == null || !chapter.SubModule.Module.Teachers.Contains(user))
            throw new ForbiddenException("User do not have permission");
        
    }

    public async Task CheckStudentChapterPermission(Guid studentId, Guid chapterId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == studentId);
        if (user == null)
            throw new NotFoundException("User not found");
        var chapter = await _dbContext.Chapters
            .Include(c=>c.SubModule)
            .ThenInclude(s=>s.Module)
            .ThenInclude(m=>m.UserModules)!                             //<---------------------
            .ThenInclude(u=>u.Student)            
            .FirstOrDefaultAsync(u => u.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");    
        if (!chapter.SubModule.Module.UserModules!.Any(u => u.Student == user && u.ModuleStatus != ModuleStatusType.InCart))
            throw new ForbiddenException("User do not have permission");
        
    }

    public async Task CheckCreatorTestPermission(Guid creatorId, Guid testId)
    {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var test = await _dbContext.Tests
            .Include(c => c.Chapter)
            .ThenInclude(f => f.SubModule)
            .ThenInclude(s => s.Module)
            .ThenInclude(m=>m.Editors)
            .FirstOrDefaultAsync(u => u.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");
        if (test.Chapter.SubModule.Module.Editors != null && !test.Chapter.SubModule.Module.Editors.Contains(user))
            throw new ForbiddenException("User do not have permission");

    }

    public async Task CheckTeacherTestPermission(Guid teacherId, Guid testId)
    {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == teacherId);
        if (user == null)
            throw new NotFoundException("User not found");
        var test = await _dbContext.Tests
            .Include(c => c.Chapter)
            .ThenInclude(c => c.SubModule)
            .ThenInclude(s => s.Module)
            .ThenInclude(m => m.Teachers)
            .FirstOrDefaultAsync(u => u.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");
        if (test.Chapter.SubModule.Module.Teachers == null || !test.Chapter.SubModule.Module.Teachers.Contains(user))
            throw new ForbiddenException("User do not have permission");
    }

    public async Task CheckStudentTestPermission(Guid studentId, Guid testId)
    {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == studentId);
        if (user == null)
            throw new NotFoundException("User not found");
        var test = await _dbContext.Tests
            .Include(c => c.Chapter)
            .ThenInclude(c => c.SubModule)
            .ThenInclude(s => s.Module)
            .ThenInclude(m => m.UserModules)!                             //<---------------------
            .ThenInclude(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == testId);
        if (test == null)
            throw new NotFoundException("Test not found");
        if (!test.Chapter.SubModule.Module.UserModules!.Any(u => u.Student == user && u.ModuleStatus != ModuleStatusType.InCart))
            throw new ForbiddenException("User do not have permission");
    }

    public async Task CheckCreatorChapterBlockPermission(Guid creatorId, Guid chapterBlockId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var chapterBlock = await _dbContext.ChapterBlocks
            .Include(cb=>cb.Chapter)
            .ThenInclude(c=>c.SubModule)
            .ThenInclude(s=>s.Module)
            .ThenInclude(m=>m.Editors)
            .FirstOrDefaultAsync(u => u.Id == chapterBlockId);
        if (chapterBlock == null)
            throw new NotFoundException("Chapter block not found");    
        if (chapterBlock.Chapter.SubModule.Module.Editors != null && !chapterBlock.Chapter.SubModule.Module.Editors.Contains(user))
            throw new ForbiddenException("User do not have permission");
        
    }

    public async Task CheckTeacherUserAnswerPermission(Guid teacherId, Guid userAnswerId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == teacherId);
        if (user == null)
            throw new NotFoundException("User not found");
        var userAnswer = await _dbContext.UserAnswers
            .Include(ua=>ua.UserAnswerTest)
            .ThenInclude(uat=>uat.Test )
            .ThenInclude(c => c.Chapter)
            .ThenInclude(c => c.SubModule)
            .ThenInclude(s => s.Module)
            .ThenInclude(m => m.Teachers)
            .FirstOrDefaultAsync(u => u.Id == userAnswerId);
        if (userAnswer?.UserAnswerTest?.Test == null)
            throw new NotFoundException("Test not found");
        if (userAnswer.UserAnswerTest.Test.Chapter.SubModule.Module.Teachers == null || !userAnswer.UserAnswerTest.Test.Chapter.SubModule.Module.Teachers!.Contains(user))
            throw new ForbiddenException("User do not have permission");
    }
}