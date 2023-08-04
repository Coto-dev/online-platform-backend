namespace HW.Common.DataTransferObjects; 

public class ModuleFullTeacherDto {
    public Guid Id { get; set; }
    public List<SubModuleFullDto> SubModules { get; set; } = new();
}