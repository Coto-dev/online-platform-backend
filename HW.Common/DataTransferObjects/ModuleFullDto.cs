namespace HW.Common.DataTransferObjects; 

public class ModuleFullDto {
    public Guid Id { get; set; }
    public float Progress { get; set; }
    public List<SubModuleFullDto> SubModules { get; set; } = new();
    public Guid? FirstUnlearnedChapter { get;set; }

}