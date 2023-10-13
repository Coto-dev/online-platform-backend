namespace HW.Common.DataTransferObjects; 

public class TestForReview {
    public Guid UserAnswerId { get; set; }
    public string ChapterName { get; set; }
    public string Question { get; set; }
    public List<string>? Files { get; set; } = new();
    public string StudentAnswerContent { get; set; }
}