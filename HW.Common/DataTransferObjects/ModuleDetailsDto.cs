using HW.Common.Enums;

namespace HW.Common.DataTransferObjects;

public class ModuleDetailsDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Price { get; set; }
    public ModuleStatusType Status { get; set; }
    public ModuleType Type { get; set; }
    public List<RequiredModulesDto> RequiredModules { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? MaxStudents { get; set; }
    public string? TimeDuration { get; set; }
}