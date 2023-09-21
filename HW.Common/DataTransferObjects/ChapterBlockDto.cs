namespace HW.Common.DataTransferObjects; 

public class ChapterBlockDto {
    public string? Content { get; set; }
    public List<string> FilesUrls { get; set; } = new();
}