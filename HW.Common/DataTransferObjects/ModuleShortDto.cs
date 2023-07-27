using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleShortDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public ModuleType? Status { get; set; }
    public ModuleStatusType? ModuleStatusType { get; set; }
    /*public DateTime? StartAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public int? MaxStudents { get; set; }*/
}