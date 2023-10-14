namespace HW.Common.DataTransferObjects; 

public class ChapterForReview {
    public List<TestForReview> TestForReview { get; set; } = new();
    public string ChapterName { get; set; }
}