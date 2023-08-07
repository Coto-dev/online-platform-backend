using HW.Account.DAL.Data;
using HW.Account.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HW.Account.BLL.Services; 

/// <inheritdoc cref="IAccountService"/>
public class AccountService: IAccountService {
    
    private readonly UserManager<User> _userManager;
    private readonly AccountDbContext _authDb;
    private readonly IFileService _fileService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="authDb"></param>
    /// <param name="fileService"></param>
    public AccountService(UserManager<User> userManager, AccountDbContext authDb, IFileService fileService) {
        _userManager = userManager;
        _authDb = authDb;
        _fileService = fileService;
    }
    
    public async Task<ProfileFullDto> GetMyProfile(Guid userId) {
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
            NickName = user.NickName,
            WorkExperience = new WorkExperienceDto {
                WorkExperienceInfos = user.WorkExperience.WorkExperiencesInfos?.Count != 0?
                    user.WorkExperience.WorkExperiencesInfos!
                        .OrderBy(w=>w.EndTime)
                        .Select(x=> new WorkExperienceInfoDto {
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
                    user.Education.EducationInfos?
                        .OrderBy(w=>w.EndTime)
                        .Select(x=> new EducationInfoDto {
                        Id = x.Id,
                        University = x.University,
                        Faculty = x.Faculty,
                        Specialization = x.Specialization,
                        Status = x.Status,
                        EndTime = x.EndTime
                    }).ToList() : new List<EducationInfoDto>(),
                Visibility = user.Education?.Visibility ?? ProfileVisibility.All
            },
            BirthDate = new BirthDateDto {
                Value = user.BirthDate.Value,
                Visibility = user.BirthDate.Visibility
            },
            JoinedAt = user.JoinedAt,
            Avatar = user.AvatarId == null ? null : new FileLinkDto() {
                FileId = user.AvatarId,
                Url = await _fileService.GetAvatarLink(user.AvatarId)
            },
            Roles = await _userManager.GetRolesAsync(userM!)
        };
        return profile;
    }

    public async Task EditProfile(Guid userId, ProfileEditDto accountProfileEditDto) {
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
        user.NickName = accountProfileEditDto.NickName;
        user.BirthDate.Value = accountProfileEditDto.BirthDate;
        user.AvatarId = accountProfileEditDto.AvatarId;
        user.Location.Place = accountProfileEditDto.Location;
        _authDb.UpdateRange(user);
         await _authDb.SaveChangesAsync();
    }

    public async Task EditProfilePrivacy(Guid userId, ProfilePrivacyEditDto profilePrivacyEditDto) {
        var user = await _authDb.Users
            .Include(u =>u.WorkExperience)
            .Include(u=>u.Location)
            .Include(u=>u.Education)
            .Include(u=>u.BirthDate)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        user.BirthDate.Visibility = profilePrivacyEditDto.BirthDateVisibility;
        user.Education.Visibility = profilePrivacyEditDto.EducationVisibility;
        user.Location.Visibility = profilePrivacyEditDto.LocationVisibility;
        user.WorkExperience.Visibility = profilePrivacyEditDto.WorkExperienceVisibility;
        _authDb.UpdateRange(user);
        await _authDb.SaveChangesAsync();
    }

    public async Task<ProfileUserFullDto> GetUserFullProfile(Guid targetUserId , Guid userId) {
        var requester = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (requester == null) {
            throw new NotFoundException("Requester not found");
        }
        var requesterRoles = await _userManager.GetRolesAsync(requester);
        var userTarget = await _userManager.FindByIdAsync(targetUserId.ToString());
        var user = await _authDb.Users
            .Include(u =>u.WorkExperience)
            .ThenInclude(w=>w.WorkExperiencesInfos)
            .Include(u=>u.Location)
            .Include(u=>u.Education)
            .ThenInclude(e=>e.EducationInfos)
            .Include(u=>u.BirthDate)
            .FirstOrDefaultAsync(u => u.Id == targetUserId);
            
        if (user == null) {
            throw new NotFoundException("User not found");
        }

        var workExp = !user.WorkExperience.WorkExperiencesInfos.IsNullOrEmpty()
            ? user.WorkExperience.WorkExperiencesInfos!
                .OrderBy(w => w.EndTime)
                .Select(x => new WorkExperienceInfoDto {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    IsContinueNowDays = x.IsContinueNowDays
                }).ToList()
            : new List<WorkExperienceInfoDto>();
        var education = !user.Education.EducationInfos.IsNullOrEmpty()
            ? user.Education.EducationInfos?
                .OrderBy(w => w.EndTime)
                .Select(x => new EducationInfoDto {
                    Id = x.Id,
                    University = x.University,
                    Faculty = x.Faculty,
                    Specialization = x.Specialization,
                    Status = x.Status,
                    EndTime = x.EndTime
                }).ToList()
            : new List<EducationInfoDto>();
        
        var profile = new ProfileUserFullDto {
            Id = user.Id,
            FullName = user.FullName,
            NickName = user.NickName,
            WorkExperienceInfos = 
               user.WorkExperience.Visibility switch{
                   ProfileVisibility.All => workExp,
                   ProfileVisibility.OnlyMe => new List<WorkExperienceInfoDto>(),
                   ProfileVisibility.OnlyTeachers => requesterRoles.Contains(ApplicationRoleNames.Teacher) 
                       ? workExp : new List<WorkExperienceInfoDto>(), 
                   _ => new List<WorkExperienceInfoDto>() 
        },
            Location = user.Location.Visibility switch {
                ProfileVisibility.All => user.Location.Place,
                ProfileVisibility.OnlyMe => null,
                ProfileVisibility.OnlyTeachers => requesterRoles.Contains(ApplicationRoleNames.Teacher) 
                    ? user.Location.Place : null, 
                _ => null
            },
            EducationInfos = user.Education.Visibility switch{
                ProfileVisibility.All => education,
                ProfileVisibility.OnlyMe => new List<EducationInfoDto>(),
                ProfileVisibility.OnlyTeachers => requesterRoles.Contains(ApplicationRoleNames.Teacher) 
                    ? education : new List<EducationInfoDto>(), 
                _ => new List<EducationInfoDto>() 
            },
            BirthDate = user.BirthDate.Visibility switch {
                ProfileVisibility.All => user.BirthDate.Value,
                ProfileVisibility.OnlyMe => null,
                ProfileVisibility.OnlyTeachers => requesterRoles.Contains(ApplicationRoleNames.Teacher) 
                    ? user.BirthDate.Value : null, 
                _ => null
            },
            AvatarUrl = user.AvatarId == null ? null : await _fileService.GetAvatarLink(user.AvatarId),
            Roles = await _userManager.GetRolesAsync(userTarget!)
        };
         return profile;
    }

    public async Task<ProfileShortDto> GetUserShortProfile(Guid userId) {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileShortDto {
            Id = user.Id,
            AvatarId = user.AvatarId == null ? null : await _fileService.GetAvatarLink(user.AvatarId),
            NickName = user.NickName
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
            Status = model.Status,
            EndTime = model.EndTime
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
        educationInfo.EndTime = model.EndTime;
        
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