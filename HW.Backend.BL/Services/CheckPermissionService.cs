using HW.Backend.DAL.Data;
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

    public async Task CheckCreatorModulePermission(Guid creatorId, Guid moduleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var module = await _dbContext.Modules
            .Include(m=>m.Creator)
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (module.Creator != user)
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
            .Include(m=>m.Students)
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");
        if (module.Students == null || !module.Students.Contains(user))
            throw new ForbiddenException("User do not have permission");
    }

    public async Task CheckCreatorSubModulePermission(Guid creatorId, Guid subModuleId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var subModule = await _dbContext.SubModules
            .Include(s=>s.Module)
            .ThenInclude(m=>m.Creator)
            .FirstOrDefaultAsync(u => u.Id == subModuleId);
        if (subModule == null)
            throw new NotFoundException(" Sub module not found");    
        if (subModule.Module.Creator != user)
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
            .ThenInclude(m=>m.Students)
            .FirstOrDefaultAsync(u => u.Id == subModuleId);
        if (subModule == null)
            throw new NotFoundException("Sub module not found");    
        if (subModule.Module.Students == null || !subModule.Module.Students.Contains(user))
            throw new ForbiddenException("User do not have permission");    }

    public async Task CheckCreatorChapterPermission(Guid creatorId, Guid chapterId) {
        var user = await _dbContext.Teachers
            .FirstOrDefaultAsync(u => u.Id == creatorId);
        if (user == null)
            throw new NotFoundException("User not found");
        var chapter = await _dbContext.Chapters
            .Include(c=>c.SubModule)
            .ThenInclude(s=>s.Module)
            .ThenInclude(m=>m.Creator)
            .FirstOrDefaultAsync(u => u.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");    
        if (chapter.SubModule.Module.Creator != user)
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
            .ThenInclude(m=>m.Students)
            .FirstOrDefaultAsync(u => u.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter not found");    
        if (chapter.SubModule.Module.Students == null || !chapter.SubModule.Module.Students.Contains(user))
            throw new ForbiddenException("User do not have permission");
        
    }
}