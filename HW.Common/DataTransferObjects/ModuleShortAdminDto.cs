using HW.Common.Enums;

namespace HW.Common.DataTransferObjects;

public class ModuleShortAdminDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Price { get; set; }
    public string? AvatarId { get; set; }
    public ModuleType? Status { get; set; }
    public ModuleStatusType? ModuleStatusType { get; set; }
    public string? TimeDuration { get; set; }
    public DateTime? StartDate { get; set; }
    public ModuleVisibilityType? Visibility { get; set; }
}