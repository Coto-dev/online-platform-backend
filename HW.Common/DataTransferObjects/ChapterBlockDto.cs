namespace HW.Common.DataTransferObjects; 

public class ChapterBlockDto {
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public List<string> FilesUrls { get; set; } = new();
}