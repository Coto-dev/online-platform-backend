using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ProfilePrivacyEditDto {
    [Required]
    public ProfileVisibility BirthDateVisibility { get; set; } = ProfileVisibility.All;
    [Required]
    public ProfileVisibility LocationVisibility { get; set; } = ProfileVisibility.All;
    [Required]
    public ProfileVisibility WorkExperienceVisibility { get; set; } = ProfileVisibility.All;
    [Required]
    public ProfileVisibility EducationVisibility { get; set; } = ProfileVisibility.All;
}