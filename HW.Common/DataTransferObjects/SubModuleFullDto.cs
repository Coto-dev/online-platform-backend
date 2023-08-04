namespace HW.Common.DataTransferObjects; 

public class SubModuleFullDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ChapterShrotDto> Chapters { get; set; } = new();
}