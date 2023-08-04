using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class WorkExperienceInfoCreateDto {
    [Required]
    public string? CompanyName { get; set; }
    [Required]
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; } = null;
    [Required] public bool IsContinueNowDays { get; set; } = false;
}