namespace HW.Common.DataTransferObjects; 

public class ModuleFullDto {
    public Guid Id { get; set; }
    public string Progress { get; set; }
    public List<SubModuleFullDto> SubModules { get; set; } = new();
    public ChapterFullDto? FirstChapter { get; set; }
}