using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleShortDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public ModuleStatusType Status { get; set; }
}