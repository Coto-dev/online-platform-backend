using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces; 

public interface IAccountService {
    /// <summary>
    /// Get user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ProfileFullDto> GetProfileAsync(Guid userId);
    
    /// <summary>
    /// Edit user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountProfileEditDto"></param>
    /// <returns></returns>
    Task EditProfileAsync(Guid userId, ProfileEditDto accountProfileEditDto);
    
    /// <summary>
    /// Get short user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ProfileShortDto> GetShortProfileAsync(Guid userId);

}