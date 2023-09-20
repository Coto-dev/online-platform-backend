namespace HW.Common.Interfaces;

public interface IAdminPanelService
{
    public Task AddTeacherRightsToUser(Guid userId, Guid moduleId);
    public Task DeleteTeacherRightsFromUser(Guid userId, Guid moduleId);
    public Task AddEditorRightsToUser(Guid userId, Guid moduleId);
    public Task DeleteEditorRightsFromUser(Guid userId, Guid moduleId);
    public Task AddStudentToModule(Guid userId, Guid moduleId);
    public Task DeleteStudentFromModule(Guid userId, Guid moduleId);
    public Task GetStudentMarksFromModule(Guid userId, Guid moduleId);
    public Task GetStudentsMarksFromModule(Guid moduleId);
    public Task BanUser(Guid userId);
    public Task UnbanUser(Guid userId);
}