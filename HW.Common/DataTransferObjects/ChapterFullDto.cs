using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ChapterFullDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; } // html
    public List<string> FileUrls { get; set; } = new();
    public List<ChapterCommentDto> Comments { get; set; } = new();
    public bool IsLearned { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsCanCheckAnswer { get; set; }
    public ChapterType ChapterType { get; set; }
    public List<TestDto>? Tests { get; set; } = new();
    public List<ChapterBlockDto>? ChapterBlocks { get; set; } = new();
}