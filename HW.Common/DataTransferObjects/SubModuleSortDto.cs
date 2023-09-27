namespace HW.Common.DataTransferObjects; 

public class SubModuleSortDto {
    public Guid Id { get; set; }
    public List<Guid> ChapterIds { get; set; } = new();
}