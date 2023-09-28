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
        SearchType? sortUserType, string? searchString)
    {
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");

        var users = _userManager.Users
            .OrderBy(u => sortUserType == SearchType.FullName ? u.FullName : u.Email)
            .Where(u => searchString == null || (sortUserType == SearchType.FullName
                ? u.FullName!.ToLower().Contains(searchString.ToLower())
                : u.Email!.ToLower().Contains(searchString.ToLower())))
            .AsNoTracking();

        var shortUsers =  users.Select( user => new UserShortDto
        {
            Id = user.Id,
            FullName = user.FullName,
            NickName = user.NickName,
            AvatarId = user.AvatarId,
            Email = user.Email!,
            IsEmailConfirm = user.EmailConfirmed,
        }).ToList();
        var ListWithRoles = new List<UserShortDto>();
        foreach (var userShortDto in shortUsers) {
            var user = await _userManager.FindByIdAsync(userShortDto.Id.ToString());
            var roles = await _userManager.GetRolesAsync(user!);
            if (roles.All(r => !roleFilter!.RoleTypes!.Select(rt => rt.ToString()).Contains(r)))
                continue;
            else {
                userShortDto.Role = roles.Contains(ApplicationRoleNames.Administrator)
                    ? ApplicationRoleNames.Administrator
                    : roles.Contains(ApplicationRoleNames.Teacher)
                        ? ApplicationRoleNames.Teacher
                        : roles.Contains(ApplicationRoleNames.Student)
                            ? ApplicationRoleNames.Student
                            : null;
                ListWithRoles.Add(userShortDto);
            }
        }
        
        var response = await PagedListObsolete<UserShortDto>.ToPagedList(ListWithRoles, pagination.PageNumber, pagination.PageSize);
        foreach (var userShortDto in response.Items)
        {
            userShortDto.AvatarId = userShortDto.AvatarId == null
                ? userShortDto.AvatarId
                : await _fileService.GetAvatarLink(userShortDto.AvatarId);
            userShortDto.IsBanned =
                await _userManager.IsLockedOutAsync((await _userManager.FindByIdAsync(userShortDto.Id.ToString()))!);
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

        var user = await _backendDbContext.UserBackends
            .Include(u=>u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) {
            var newUser = new UserBackend() {
                Id = userId
            };
            user = newUser;
            await _backendDbContext.AddAsync(user);
        }
        user.Teacher ??= new Teacher {
            Id = user.Id,
            UserBackend = user
        };

        var result = await _userManager.UpdateAsync(userM);
        if (result.Succeeded)
            await _backendDbContext.SaveChangesAsync();
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
            .FirstOrDefaultAsync(t => t.Id == userId);
        if (teacher != null) {
            _backendDbContext.Remove(teacher);
            await _backendDbContext.SaveChangesAsync();
        }
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
            .Include(u=>u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) {
            var newUser = new UserBackend() {
                Id = userId
            };
            user = newUser;
            await _backendDbContext.AddAsync(user);
        }
        user.Teacher ??= new Teacher {
            Id = user.Id,
            UserBackend = user
        };

        var module = await _backendDbContext.Modules
            .Include(t => t.Teachers)!
            .FirstOrDefaultAsync(m => m.Id == moduleId)
            ?? throw new NotFoundException("Module with this Id not found.");
        if (module.Teachers!.Contains(user.Teacher))
            throw new ConflictException("User is already teacher on this module");

        if (module.Teachers == null)
        {
            module.Teachers = new List<Teacher> { user.Teacher };
        }
        else
        {
            module.Teachers.Add(user.Teacher);
        }

        _backendDbContext.Update(module);
        await _backendDbContext.SaveChangesAsync();
    }

    public async Task DeleteTeacherRightsFromUserOnModule(Guid userId, Guid moduleId)
    {
        var user = await _backendDbContext.UserBackends
            .Include(t => t.Teacher)
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
                         .Include(e => e.Teachers)!
                         .FirstOrDefaultAsync(m => m.Id == moduleId)
                     ?? throw new NotFoundException("Module with this Id not found.");

        if (user.Teacher == null || !module.Teachers!.Contains(user.Teacher))
            throw new ConflictException("User is not teacher");
        
        module.Teachers.Remove(user.Teacher);
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
            .Include(u=>u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) {
            var newUser = new UserBackend() {
                Id = userId
            };
            user = newUser;
            await _backendDbContext.AddAsync(user);
        }
        user.Teacher ??= new Teacher {
            Id = user.Id,
            UserBackend = user
        };

        var module = await _backendDbContext.Modules
                         .Include(t => t.Editors)!
                         .FirstOrDefaultAsync(m => m.Id == moduleId)
                     ?? throw new NotFoundException("Module with this Id not found.");
        
        if (module.Teachers!.Contains(user.Teacher))
            throw new ConflictException("User already editor");

        if (module.Editors == null)
        {
            module.Editors = new List<Teacher> { user.Teacher };
        }
        else
        {
            module.Editors.Add(user.Teacher);
        }

        _backendDbContext.Update(module);
        await _backendDbContext.SaveChangesAsync();
      
    }

    public async Task DeleteEditorRightsFromUserOnModule(Guid userId, Guid moduleId)
    {
        var user = await _backendDbContext.UserBackends
            .Include(t => t.Teacher)!
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException("User with this Id not found.");

        var module = await _backendDbContext.Modules
            .Include(e => e.Editors)!
            .FirstOrDefaultAsync(m => m.Id == moduleId)
            ?? throw new NotFoundException("Module with this Id not found.");

        if (user.Teacher == null || !module.Editors!.Contains(user.Teacher))
            throw new ConflictException("User is not editor");
        
        module.Editors.Remove(user.Teacher);
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