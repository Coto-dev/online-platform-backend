using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ChapterFullTeacherDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; } // html
    public List<FileLinkDto> FileIds { get; set; } = new();
    public List<ChapterCommentDto> Comments { get; set; } = new();
    public ChapterType ChapterType { get; set; }
    public List<TestTeacherDto>? Tests { get; set; } = new();
    public List<ChapterBlockTeacherDto>? ChapterBlocks { get; set; } = new();
}