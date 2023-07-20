using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Account.API.Controllers; 

/// <summary>
/// Account controller
/// </summary>
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase {
    private readonly IAccountService _accountService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accountService"></param>
    public AccountController(IAccountService accountService) {
        _accountService = accountService;
    }
    /// <summary>
    /// Get information about current authenticated user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ProfileFullDto>> GetCurrentProfile() {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _accountService.GetProfileAsync(userId));
    }
    
    /// <summary> 
    /// Edit current authenticated user`s profile
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> UpdateProfile([FromBody] ProfileEditDto profileEditDto) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _accountService.EditProfileAsync(userId, profileEditDto);
        return Ok();
    }
    /// <summary>
    /// Get short information about current authenticated user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{userId}")]
    public async Task<ActionResult<ProfileShortDto>> GetCurrentProfile(Guid userId) {
        return Ok(await _accountService.GetShortProfileAsync(userId));
    }

    /// <summary>
    /// Add education info 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("education-info")]
    public async Task<ActionResult> AddEducationInfo(EducationInfoCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _accountService.AddEducationInfo(model, userId);
        return Ok();
    }
    
    /// <summary>
    /// Edit education info
    /// </summary>
    /// <param name="model"></param>
    /// <param name="educationInfoId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("education-info/{educationInfoId}")]
    public async Task<ActionResult> EditEducationInfo(EducationInfoCreateDto model , Guid educationInfoId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _accountService.EditEducationInfo(model, educationInfoId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Delete education info
    /// </summary>
    /// <param name="educationInfoId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("education-info/{educationInfoId}")]
    public async Task<ActionResult> DeleteEducationInfo(Guid educationInfoId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _accountService.DeleteEducationInfo(educationInfoId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Add work experience
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("work-experience")]
    public async Task<ActionResult> AddWorkExperience(WorkExperienceInfoCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _accountService.AddWorkExperience(model, userId);
        return Ok();
    }
    
    /// <summary>
    /// Edit work experience
    /// </summary>
    /// <param name="model"></param>
    /// <param name="workExperienceInfoId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("work-experience/{workExperienceInfoId}")]
    public async Task<ActionResult> EditWorkExperience(WorkExperienceInfoCreateDto model , Guid workExperienceInfoId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _accountService.EditWorkExperience(model, workExperienceInfoId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Delete work experience
    /// </summary>
    /// <param name="workExperienceInfoId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("work-experience/{workExperienceInfoId}")]
    public async Task<ActionResult> DeleteWorkExperience(Guid workExperienceInfoId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _accountService.DeleteWorkExperience(workExperienceInfoId, userId);
        return Ok();
    }
}
