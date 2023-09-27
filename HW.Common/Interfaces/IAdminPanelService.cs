using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;

namespace HW.Common.Interfaces;

public interface IAdminPanelService
{
    public Task<PagedList<UserShortDto>> GetUsers(PaginationParamsDto pagination, FilterRoleType? filter,
        SortUserType? sortUserType);
    public Task<PagedList<ModuleShortAdminDto>> GetModules(PaginationParamsDto pagination, FilterModuleType? filter,
            string? sortByNameFilter, SortModuleType? sortModuleType);
    public Task AddTeacherRightsToUser(Guid userId);
    public Task DeleteTeacherRightsFromUser(Guid userId);
    public Task AddTeacherRightsToUserOnModule(Guid userId, Guid moduleId);
    public Task DeleteTeacherRightsFromUserOnModule(Guid userId, Guid moduleId);
    public Task AddEditorRightsToUserOnModule(Guid userId, Guid moduleId);
    public Task DeleteEditorRightsFromUserOnModule(Guid userId, Guid moduleId);
    public Task AddStudentToModule(Guid userId, Guid moduleId);
    public Task DeleteStudentFromModule(Guid userId, Guid moduleId);
    public Task GetStudentMarksFromModule(Guid userId, Guid moduleId);
    public Task GetStudentsMarksFromModule(Guid moduleId);
    public Task BanUser(Guid userId);
    public Task UnbanUser(Guid userId);
}