using HW.Backend.DAL.Data;
using HW.Common.Enums;
using HW.Backend.DAL.Data.Entities;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HW.Common.Other;
using HW.Common.DataTransferObjects;
using HW.Account.DAL.Data.Entities;
using HW.Account.DAL.Data;
using Microsoft.AspNetCore.Identity;
using HW.AdminPanel.BL.Extensions;
using System.Linq;

namespace HW.AdminPanel.BL.Services;
    
public class AdminPanelService : IAdminPanelService
{
    private readonly ILogger<AdminPanelService> _logger;
    private readonly BackendDbContext _backendDbContext;
    private readonly AccountDbContext _accountDbContext;
    private readonly IFileService _fileService;
    private readonly UserManager<User> _userManager;

    public AdminPanelService(ILogger<AdminPanelService> logger, BackendDbContext backendDbContext, AccountDbContext accountDbContext,
        IFileService fileService, UserManager<User> userManager)
    {
        _logger = logger;
        _backendDbContext = backendDbContext;
        _accountDbContext = accountDbContext;
        _fileService = fileService;
        _userManager = userManager;
    }

    public async Task<PagedList<UserShortDto>> GetUsers(PaginationParamsDto pagination, FilterRoleType? roleFilter,
        SortUserType? sortUserType)
    {
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");

        var users = _userManager.Users
            .OrderBy(s => sortUserType == SortUserType.Name ? s.FullName : s.Email)
            .AsQueryable()
            .AsNoTracking();

        var shortUsers = users.Select(user => new UserShortDto
        {
            Id = user.Id,
            FullName = user.FullName,
            NickName = user.NickName,
            AvatarId = user.AvatarId,
            Email = user.Email!,
            IsEmailConfirm = user.EmailConfirmed
        });

        var response = await PagedList<UserShortDto>.ToPagedList(shortUsers, pagination.PageNumber, pagination.PageSize);
        foreach (var userShortDto in response.Items)
        {
            userShortDto.AvatarId = userShortDto.AvatarId == null
                ? userShortDto.AvatarId
                : await _fileService.GetAvatarLink(userShortDto.AvatarId);

            var user = await _userManager.FindByIdAsync(userShortDto.Id.ToString());
            userShortDto.Roles = await _userManager.GetRolesAsync(user!);
            userShortDto.IsBanned = await _userManager.IsLockedOutAsync(user!);
        }
        return response;
    }

    public async Task<PagedList<ModuleShortAdminDto>> GetModules(PaginationParamsDto pagination, FilterModuleType? filter,
            string? sortByNameFilter, SortModuleType? sortModuleType)
    {
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");

        var modules = _backendDbContext.Modules
            .ModuleAdminFilter(filter, sortByNameFilter)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();

        var shortModules = modules.Select(x => new ModuleShortAdminDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            AvatarId = x.AvatarId,
            TimeDuration = x.TimeDuration,
            Visibility = x.ModuleVisibility,
            Status = typeof(Module) == x.GetType() ? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
        });
        var response = await PagedList<ModuleShortAdminDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
        foreach (var moduleShortDto in response.Items)
        {
            moduleShortDto.AvatarId = moduleShortDto.AvatarId == null
                ? moduleShortDto.AvatarId
                : await _fileService.GetAvatarLink(moduleShortDto.AvatarId);
        }
        return response;
    }

    public async Task AddTeacherRightsToUser(Guid userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(ApplicationRoleNames.Teacher))
        {
            await _userManager.AddToRoleAsync(userM, ApplicationRoleNames.Teacher);
        };

        await _userManager.UpdateAsync(userM);
    }

    public async Task DeleteTeacherRightsFromUser(Guid  userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(ApplicationRoleNames.Teacher))
        {
            throw new BadRequestException("User doesn't have teacher permissions.");
        }
        else
        {
            await _userManager.RemoveFromRoleAsync(userM, ApplicationRoleNames.Teacher);
            await _userManager.UpdateAsync(userM);
        }

        var teacher = await _backendDbContext.Teachers
            .FirstOrDefaultAsync(t => t.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        _backendDbContext.Remove(teacher);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task AddTeacherRightsToUserOnModule(Guid userId, Guid moduleId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(RoleType.Teacher.ToString()))
        {
            await _userManager.AddToRoleAsync(userM, ApplicationRoleNames.Teacher);
            await _userManager.UpdateAsync(userM);
        };

        var user = await _backendDbContext.UserBackends
            .Include(t => t.Teacher)!
            .ThenInclude(m => m.ControlledModules)!
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(t => t.Teachers)!
            .FirstOrDefaultAsync(m => m.Id == moduleId)
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

        _backendDbContext.Update(user);
        _backendDbContext.Update(module);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task DeleteTeacherRightsFromUserOnModule(Guid userId, Guid moduleId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(RoleType.Teacher.ToString()))
        {
            throw new BadRequestException("User doesn't have teacher permissions.");
        }

        var user = await _backendDbContext.UserBackends
            .Include(t => t.Teacher)!
            .ThenInclude(m => m.ControlledModules)!
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(t => t.Teachers)!
            .FirstOrDefaultAsync(m => m.Id == moduleId)
            ?? throw new NotFoundException("Module with this Id not found.");

        if (user.Teacher.ControlledModules == null || !user.Teacher.ControlledModules.Contains(module))
        {
            throw new BadRequestException("User doesn't have teacher permissions on this module.");
        }
        else
        {
            user.Teacher.ControlledModules.Remove(module);
            module.Teachers.Remove(user.Teacher);
        };

        _backendDbContext.Update(user);
        _backendDbContext.Update(module);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task AddEditorRightsToUserOnModule(Guid userId, Guid moduleId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(RoleType.Teacher.ToString()))
        {
            await _userManager.AddToRoleAsync(userM, ApplicationRoleNames.Teacher);
            await _userManager.UpdateAsync(userM);
        };

        var user = await _backendDbContext.UserBackends
            .Include(t => t.Teacher)!
            .ThenInclude(m => m.ControlledModules)!
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(t => t.Teachers)!
            .Include(e => e.Editors)!
            .FirstOrDefaultAsync(m => m.Id == moduleId)
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

        if (module.Editors == null)
        {
            module.Editors = new List<Teacher> { user.Teacher };
        }
        else
        {
            module.Editors.Add(user.Teacher);
        }
        _backendDbContext.Update(user);
        _backendDbContext.Update(module);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task DeleteEditorRightsFromUserOnModule(Guid userId, Guid moduleId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(RoleType.Teacher.ToString()))
        {
            throw new BadRequestException("User doesn't have teacher permissions.");
        }

        var user = await _backendDbContext.UserBackends
            .Include(t => t.Teacher)!
            .ThenInclude(m => m.ControlledModules)!
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(t => t.Teachers)!
            .Include(e => e.Editors)!
            .FirstOrDefaultAsync(m => m.Id == moduleId)
            ?? throw new NotFoundException("Module with this Id not found.");

        if (user.Teacher.ControlledModules == null || !user.Teacher.ControlledModules.Contains(module) || !module.Editors.Contains(user.Teacher))
        {
            throw new BadRequestException("User doesn't have teacher permissions on this module.");
        }
        else
        {
            module.Editors.Remove(user.Teacher);
        };

        _backendDbContext.Update(module);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task AddStudentToModule(Guid userId, Guid moduleId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(ApplicationRoleNames.Student))
        {
            await _userManager.AddToRoleAsync(userM, ApplicationRoleNames.Student);
            await _userManager.UpdateAsync(userM);
        };

        var user = await _backendDbContext.UserBackends
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(m => m.UserModules)
            .FirstOrDefaultAsync(s => s.Id == moduleId) ?? throw new NotFoundException("Module with this Id not found.");

        user.Student ??= new Student
            {
                Id = userId,
                UserBackend = user
            };

        var studentModule = module.UserModules.Find(m => m.Module == module && m.Student == user.Student);

        if (studentModule != null)
        {
            throw new BadRequestException("Student already have this module.");
        }
        else
        {
            var newStudentModule = new StudentModule
            {
                Id = new Guid(),
                Module = module,
                Student = user.Student,
                ModuleStatus = ModuleStatusType.Purchased
            };

            _backendDbContext.UserModules.Add(newStudentModule);
            await _backendDbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteStudentFromModule(Guid userId, Guid moduleId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");
        var userRoles = await _userManager.GetRolesAsync(userM);

        if (!userRoles.Contains(ApplicationRoleNames.Student))
        {
            throw new NotFoundException("User dont have student permissions.");
        };

        var user = await _backendDbContext.UserBackends
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(m => m.UserModules)
            .FirstOrDefaultAsync(s => s.Id == moduleId) ?? throw new NotFoundException("Module with this Id not found.");

        var studentModule = module.UserModules.Find(m => m.Module == module && m.Student == user.Student) 
            ?? throw new NotFoundException("Student dont have this module.");

        _backendDbContext.UserModules.Remove(studentModule);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task GetStudentMarksFromModule(Guid userId, Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public async Task GetStudentsMarksFromModule(Guid moduleId)
    {
        throw new NotImplementedException();
    }

    public async Task BanUser(Guid userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");

        if (userM.LockoutEnabled)
        {
            DateTimeOffset lockoutDate = new DateTimeOffset(DateTime.UtcNow.AddYears(50));
            await _userManager.SetLockoutEndDateAsync(userM, lockoutDate);
            await _userManager.UpdateAsync(userM);
        }
        else
        {
            throw new BadRequestException("User cannot be blocked.");
        }
    }

    public async Task UnbanUser(Guid userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");

        if (userM.LockoutEnabled)
        {
            await _userManager.SetLockoutEndDateAsync(userM, null);
            await _userManager.UpdateAsync(userM);
        }
        else
        {
            throw new BadRequestException("User cannot be unblocked.");
        }
    }
}