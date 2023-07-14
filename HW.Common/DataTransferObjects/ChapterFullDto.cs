namespace HW.Common.DataTransferObjects; 

public class ChapterFullDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; } // html
    public List<Guid> FileIds { get; set; } = new();
    public List<ChapterCommentDto> Comments { get; set; } = new();
    public bool IsLearned { get; set; }
    public bool IsTestChapter { get; set; }
    public List<TestDto>? Tests { get; set; }
}