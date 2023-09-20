using HW.Backend.DAL.Data;
using HW.Common.Enums;
using HW.Backend.DAL.Data.Entities;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HW.AdminPanel.BL.Services;

public class AdminPanelService : IAdminPanelService
{
    private readonly ILogger<AdminPanelService> _logger;
    private readonly BackendDbContext _dbContext;

    public AdminPanelService(ILogger<AdminPanelService> logger, BackendDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task AddTeacherRightsToUser(Guid userId, Guid moduleId)
    {
        var user = await _dbContext.UserBackends
            .Include(t => t.Teacher)!
            .ThenInclude(m => m.ControlledModules)!
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _dbContext.Modules
            .Include(t => t.Teachers)!
            .FirstOrDefaultAsync(m => m.Id == moduleId && m.ModuleVisibility != ModuleVisibilityType.OnlyCreators)
            ?? throw new NotFoundException("Module with this Id not found.");

        if (user.Teacher == null)
        {
            user.Teacher = new Teacher
            {
                Id = userId,
                UserBackend = user,
                ControlledModules = new List<Module> { module },
            };
        }
        else
        {
            if (user.Teacher.ControlledModules == null)
            {
                user.Teacher.ControlledModules = new List<Module> { module };
            }
            else
            {
                user.Teacher.ControlledModules.Add(module);
            }
        }

        if (module.Teachers == null)
        {
            module.Teachers = new List<Teacher> { user.Teacher };
        }
        else
        {
            module.Teachers.Add(user.Teacher);
        }

        _dbContext.Update(user);
        _dbContext.Update(module);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteTeacherRightsFromUser(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task AddEditorRightsToUser(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteEditorRightsFromUser(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task AddStudentToModule(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStudentFromModule(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task GetStudentMarksFromModule(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task GetStudentsMarksFromModule(Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public Task BanUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task UnbanUser(Guid userId)
    {
        throw new NotImplementedException();
    }
}