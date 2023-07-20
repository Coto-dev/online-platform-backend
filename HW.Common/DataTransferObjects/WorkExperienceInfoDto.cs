using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class WorkExperienceInfoDto {
    public Guid Id { get; set; }
    public string? CompanyName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsContinueNowDays { get; set; }
}