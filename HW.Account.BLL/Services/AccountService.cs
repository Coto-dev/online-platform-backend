using HW.Account.DAL.Data;
using HW.Account.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        var userM = await _userManager.FindByIdAsync(userId.ToString());
        var user = await _authDb.Users
            .Include(u =>u.WorkExperience)
            .ThenInclude(w=>w.WorkExperiencesInfos)
            .Include(u=>u.Location)
            .Include(u=>u.Education)
            .ThenInclude(e=>e.EducationInfos)
            .Include(u=>u.BirthDate)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileFullDto {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            WorkExperience = new WorkExperienceDto {
                WorkExperienceInfos = user.WorkExperience.WorkExperiencesInfos?.Count != 0?
                    user.WorkExperience.WorkExperiencesInfos!.Select(x=> new WorkExperienceInfoDto {
                        Id = x.Id,
                        CompanyName = x.CompanyName,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        IsContinueNowDays = x.IsContinueNowDays
                    }).ToList(): new List<WorkExperienceInfoDto>(),
                Visibility = user.WorkExperience.Visibility
            },
            Location = new LocationDto {
                Place = user.Location.Place,
                Visibility = user.Location.Visibility
            },
            Education = new EducationDto {
                EducationInfos = user.Education.EducationInfos?.Count !=0?
                    user.Education.EducationInfos?.Select(x=> new EducationInfoDto {
                        Id = x.Id,
                        University = x.University,
                        Faculty = x.Faculty,
                        Specialization = x.Specialization,
                        Status = x.Status
                    }).ToList() : new List<EducationInfoDto>(),
                Visibility = user.Education?.Visibility ?? ProfileVisibility.All
            },
            BirthDate = new BirthDateDto {
                Value = user.BirthDate.Value,
                Visibility = user.BirthDate.Visibility
            },
            JoinedAt = user.JoinedAt,
            AvatarId = user.AvatarId,
            Roles = await _userManager.GetRolesAsync(userM!)
        };
        return profile;
    }

    /// <inheritdoc cref="IAccountService.EditProfileAsync"/>
    public async Task EditProfileAsync(Guid userId, ProfileEditDto accountProfileEditDto) {
        var user = await _authDb.Users
            .Include(u =>u.WorkExperience)
            .Include(u=>u.Location)
            .Include(u=>u.Education)
            .Include(u=>u.BirthDate)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        user.FullName = accountProfileEditDto.FullName;
        user.BirthDate.Value = accountProfileEditDto.BirthDate.Value;
        user.BirthDate.Visibility = accountProfileEditDto.BirthDate.Visibility;
        user.AvatarId = accountProfileEditDto.AvatarId;
        user.Education.Visibility = accountProfileEditDto.EducationVisibility;
        user.Location.Place = accountProfileEditDto.LocationDto.Place;
        user.Location.Visibility = accountProfileEditDto.LocationDto.Visibility;
        user.WorkExperience.Visibility = accountProfileEditDto.WorkExperienceVisibility;
        _authDb.UpdateRange(user);
         await _authDb.SaveChangesAsync();
    }
    
    /// <inheritdoc cref="IAccountService.GetShortProfileAsync"/>
    public async Task<ProfileShortDto> GetShortProfileAsync(Guid userId) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileShortDto {
            Id = user.Id,
            AvatarId = user.AvatarId,
            FullName = user.FullName
        };
        return profile;    
    }

    public async Task AddEducationInfo(EducationInfoCreateDto model, Guid userId) {
        var user = await _authDb.Users
            .Include(u=>u.Education)
            .ThenInclude(e=>e.EducationInfos)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        
        user.Education.EducationInfos ??= new List<EducationInfo>();
        var educationInfo = new EducationInfo {
            Education = user.Education,
            University = model.University,
            Faculty = model.Faculty,
            Specialization = model.Specialization,
            Status = model.Status
        };
        await _authDb.AddAsync(educationInfo);
        await _authDb.SaveChangesAsync();
    }

    public async Task EditEducationInfo(EducationInfoCreateDto model, Guid id, Guid userId) {
        var user = await _authDb.Users
            .Include(u=>u.Education)
            .ThenInclude(e=>e.EducationInfos)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var educationInfo = await _authDb.EducationInfos
            .FirstOrDefaultAsync(x => x.Id == id);
        if (educationInfo == null) {
            throw new NotFoundException("Education not found");
        }
        
        if (user.Education.EducationInfos == null 
            || user.Education.EducationInfos.Count == 0 
            || !user.Education.EducationInfos.Contains(educationInfo)) {
            throw new ForbiddenException("Dont have permission to do that");
        }
        educationInfo.University = model.University;
        educationInfo.Specialization = model.Specialization;
        educationInfo.Faculty = model.Faculty;
        educationInfo.Status = model.Status;
        
        await _authDb.SaveChangesAsync();
    }

    public async Task DeleteEducationInfo(Guid id, Guid userId) {
        var user = await _authDb.Users
            .Include(u=>u.Education)
            .ThenInclude(e=>e.EducationInfos)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var educationInfo = await _authDb.EducationInfos
            .FirstOrDefaultAsync(x => x.Id == id);
        if (educationInfo == null) {
            throw new NotFoundException("Education not found");
        }
        
        if (user.Education.EducationInfos == null 
            || user.Education.EducationInfos.Count == 0 
            || !user.Education.EducationInfos.Contains(educationInfo)) {
            throw new ForbiddenException("Dont have permission to do that");
        }

        _authDb.Remove(educationInfo);
        await _authDb.SaveChangesAsync();
    }

    public async Task AddWorkExperience(WorkExperienceInfoCreateDto model, Guid userId) {
        var user = await _authDb.Users
            .Include(u=>u.WorkExperience)
            .ThenInclude(e=>e.WorkExperiencesInfos)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        
        user.WorkExperience.WorkExperiencesInfos ??= new List<WorkExperienceInfo>();
        var workExperienceInfo = new WorkExperienceInfo {
            WorkExperience = user.WorkExperience,
            CompanyName = model.CompanyName,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            IsContinueNowDays = model.IsContinueNowDays
        };
        await _authDb.AddAsync(workExperienceInfo);
        await _authDb.SaveChangesAsync();    
    }

    public async Task EditWorkExperience(WorkExperienceInfoCreateDto model, Guid id, Guid userId) {
        var user = await _authDb.Users
            .Include(u=>u.WorkExperience)
            .ThenInclude(e=>e.WorkExperiencesInfos)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var workExperienceInfo = await _authDb.WorkExperienceInfos
            .FirstOrDefaultAsync(x => x.Id == id);
        if (workExperienceInfo == null) {
            throw new NotFoundException("WorkExperience not found");
        }
        
        if (user.WorkExperience.WorkExperiencesInfos == null 
            || user.WorkExperience.WorkExperiencesInfos.Count == 0 
            || !user.WorkExperience.WorkExperiencesInfos.Contains(workExperienceInfo)) {
            throw new ForbiddenException("Dont have permission to do that");
        }

        workExperienceInfo.CompanyName = model.CompanyName;
        workExperienceInfo.StartTime = model.StartTime;
        workExperienceInfo.EndTime = model.EndTime;
        workExperienceInfo.IsContinueNowDays = model.IsContinueNowDays;
        
        await _authDb.SaveChangesAsync();
    }

    public async Task DeleteWorkExperience(Guid id, Guid userId) {
        var user = await _authDb.Users
            .Include(u=>u.WorkExperience)
            .ThenInclude(e=>e.WorkExperiencesInfos)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var workExperienceInfo = await _authDb.WorkExperienceInfos
            .FirstOrDefaultAsync(x => x.Id == id);
        if (workExperienceInfo == null) {
            throw new NotFoundException("WorkExperience not found");
        }
        
        if (user.WorkExperience.WorkExperiencesInfos == null 
            || user.WorkExperience.WorkExperiencesInfos.Count == 0 
            || !user.WorkExperience.WorkExperiencesInfos.Contains(workExperienceInfo)) {
            throw new ForbiddenException("Dont have permission to do that");
        }

        _authDb.Remove(workExperienceInfo);
        await _authDb.SaveChangesAsync();
    }
}