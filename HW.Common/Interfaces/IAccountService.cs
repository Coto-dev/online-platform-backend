using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces; 

public interface IAccountService {
    Task<ProfileFullDto> GetMyProfile(Guid userId);
    Task EditProfile(Guid userId, ProfileEditDto accountProfileEditDto);
    Task EditProfilePrivacy(Guid userId, ProfilePrivacyEditDto profilePrivacyEditDto);
    Task<ProfileUserFullDto> GetUserFullProfile(Guid userId);
    Task<ProfileShortDto> GetUserShortProfile(Guid userId);
    Task AddEducationInfo(EducationInfoCreateDto model, Guid userId);
    Task EditEducationInfo(EducationInfoCreateDto model, Guid id, Guid userId);
    Task DeleteEducationInfo(Guid id, Guid userId);
    Task AddWorkExperience(WorkExperienceInfoCreateDto model, Guid userId);
    Task EditWorkExperience(WorkExperienceInfoCreateDto model, Guid id, Guid userId);
    Task DeleteWorkExperience(Guid id, Guid userId);


}