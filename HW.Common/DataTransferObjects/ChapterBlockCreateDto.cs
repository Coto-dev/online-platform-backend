namespace HW.Common.DataTransferObjects; 

public class ChapterBlockCreateDto {
    public string? Content { get; set; } 
    public List<string>? FileIds { get; set; } = new();
}