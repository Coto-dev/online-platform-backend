namespace HW.Common.DataTransferObjects; 

public class ModuleFullDto {
    public Guid Id { get; set; }
    public string Progress { get; set; }
    public List<ChapterShrotDto> Chapters { get; set; } = new();
    public ChapterFullDto? FirstChapter { get; set; }
}