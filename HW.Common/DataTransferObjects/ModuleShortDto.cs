using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleShortDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string? AvatarId { get; set; }
    public ModuleType? Status { get; set; }
    public ModuleStatusType? ModuleStatusType { get; set; }
    public float Progress { get; set; }
    public string? TimeDuration { get; set; }
    public DateTime? StartDate { get; set; }
    /*public DateTime? StartAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public int? MaxStudents { get; set; }*/
}