namespace HW.Common.DataTransferObjects; 

public class TestForReview {
    public Guid TestId { get; set; }
    public string ChapterName { get; set; }
    public string Question { get; set; }
    public string StudentAnswerContent { get; set; }
}