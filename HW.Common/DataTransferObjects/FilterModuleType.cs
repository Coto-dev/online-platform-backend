using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class FilterModuleType {
    public List<ModuleType>? ModuleTypes { get; set; } =  new List<ModuleType>((ModuleType[])Enum.GetValues(typeof(ModuleType)));
}