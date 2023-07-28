using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ChapterFullDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; } // html
    public List<FileLinkDto> FileIds { get; set; } = new();
    public List<ChapterCommentDto> Comments { get; set; } = new();
    public bool IsLearned { get; set; }
    public ChapterType ChapterType { get; set; }
    public List<TestDto>? Tests { get; set; }
}