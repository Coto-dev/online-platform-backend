namespace HW.Account.DAL.Data.Entities; 

public class WorkExperienceInfo {
    public Guid Id { get; set; } = Guid.NewGuid();
    public WorkExperience WorkExperience { get; set; }
    public string? CompanyName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsContinueNowDays { get; set; }
}