using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleTeacherFilter {
    public List<ModuleFilterTeacherType>? Sections { get; set; } =  new List<ModuleFilterTeacherType>((ModuleFilterTeacherType[])Enum.GetValues(typeof(ModuleType)));

}