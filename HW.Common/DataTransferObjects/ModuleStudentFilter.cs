using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleStudentFilter {
    public List<ModuleFilterStudentType>? ModuleStudentTypes { get; set; } =  new List<ModuleFilterStudentType>((ModuleFilterStudentType[])Enum.GetValues(typeof(ModuleType)));
}