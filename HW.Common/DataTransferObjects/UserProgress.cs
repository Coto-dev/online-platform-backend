namespace HW.Common.DataTransferObjects; 

public class UserProgress {
    public Guid Id { get; set; }
    public int PassedTests { get; set; }
    public int LearnedChapters { get; set; }
    public float Progress { get; set; }
}