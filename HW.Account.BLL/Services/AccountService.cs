using HW.Account.DAL.Data;
using HW.Account.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HW.Account.BLL.Services; 

/// <inheritdoc cref="IAccountService"/>
public class AccountService: IAccountService {
    
    private readonly UserManager<User> _userManager;
    private readonly AccountDbContext _authDb;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="authDb"></param>
    public AccountService(UserManager<User> userManager, AccountDbContext authDb) {
        _userManager = userManager;
        _authDb = authDb;
    }
    
    /// <inheritdoc cref="IAccountService.GetProfileAsync"/>
    public async Task<ProfileFullDto> GetProfileAsync(Guid userId) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileFullDto {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            WorkExperience = user.WorkExperience,
            Location = user.Location,
            Education = user.Education,
            BirthDate = user.BirthDate,
            JoinedAt = user.JoinedAt,
            PhotoId = user.PhotoId,
        };
        return profile;
    }

    /// <inheritdoc cref="IAccountService.EditProfileAsync"/>
    public async Task EditProfileAsync(Guid userId, ProfileEditDto accountProfileEditDto) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        user.FullName = accountProfileEditDto.FullName;
        user.BirthDate = accountProfileEditDto.BirthDate;
        user.PhotoId = accountProfileEditDto.PhotoId;
        user.Education = accountProfileEditDto.Education;
        user.Location = accountProfileEditDto.Location;
        user.WorkExperience = accountProfileEditDto.WorkExperience;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) {
            throw new InvalidOperationException("User update failed");
        } 
    }

    /// <inheritdoc cref="IAccountService.GetShortProfileAsync"/>
    public async Task<ProfileShortDto> GetShortProfileAsync(Guid userId) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileShortDto {
            Id = user.Id,
            PhotoId = user.PhotoId,
            FullName = user.FullName
        };
        return profile;    
    }
    
}