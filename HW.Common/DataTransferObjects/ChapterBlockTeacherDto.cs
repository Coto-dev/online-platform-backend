namespace HW.Common.DataTransferObjects; 

public class ChapterBlockTeacherDto {
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public List<FileLinkDto> FileIds { get; set; } = new();

}