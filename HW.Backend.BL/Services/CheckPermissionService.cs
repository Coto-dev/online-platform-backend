using HW.Common.Interfaces;

namespace HW.Backend.BL.Services; 

public class CheckPermissionService: ICheckPermissionService {
    public async Task CheckCreatorModulePermission(Guid creatorId, Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task CheckTeacherModulePermission(Guid teacherId, Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task CheckStudentModulePermission(Guid studentId, Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task CheckCreatorSubModulePermission(Guid creatorId, Guid subModuleId) {
        throw new NotImplementedException();
    }

    public async Task CheckTeacherSubModulePermission(Guid teacherId, Guid subModuleId) {
        throw new NotImplementedException();
    }

    public async Task CheckStudentSubModulePermission(Guid studentId, Guid subModuleId) {
        throw new NotImplementedException();
    }

    public async Task CheckCreatorChapterPermission(Guid creatorId, Guid chapterId) {
        throw new NotImplementedException();
    }

    public async Task CheckTeacherChapterPermission(Guid teacherId, Guid chapterId) {
        throw new NotImplementedException();
    }

    public async Task CheckStudentChapterPermission(Guid student, Guid chapterId) {
        throw new NotImplementedException();
    }
}