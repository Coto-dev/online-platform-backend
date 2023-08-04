using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

/// <summary>
/// Profile DTO for Edit
/// </summary>
public class ProfileEditDto {
    
    
    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    [Required]
    public required string FullName { get; set; }
    
    /// <summary>
    /// User's avatar id 
    /// </summary>
    [Required]
    public string? AvatarId { get; set; }
    /// <summary>
    /// User's work experience visibility
    /// </summary>
    [Required]
    [DefaultValue(ProfileVisibility.All)]
    public ProfileVisibility WorkExperienceVisibility { get; set; } 
    
    /// <summary>
    /// User's location(city)
    /// </summary>
    [Required]
    public LocationDto LocationDto { get; set; }
    
    /// <summary>
    /// User's education grade visibility
    /// </summary>
    [Required]
    [DefaultValue(ProfileVisibility.All)]
    public ProfileVisibility EducationVisibility { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    [Required]
    public required BirthDateDto BirthDate { get; set; }

}