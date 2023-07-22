namespace HW.Common.Interfaces; 

public interface ICheckPermissionService {
    Task CheckCreatorModulePermission(Guid creatorId, Guid moduleId);
    Task CheckTeacherModulePermission(Guid teacherId, Guid moduleId);
    Task CheckStudentModulePermission(Guid studentId, Guid moduleId);
    
    Task CheckCreatorSubModulePermission(Guid creatorId, Guid subModuleId);
    Task CheckTeacherSubModulePermission(Guid teacherId, Guid subModuleId);
    Task CheckStudentSubModulePermission(Guid studentId, Guid subModuleId);
    
    Task CheckCreatorChapterPermission(Guid creatorId, Guid chapterId);
    Task CheckTeacherChapterPermission(Guid teacherId, Guid chapterId);
    Task CheckStudentChapterPermission(Guid student, Guid chapterId);
}