namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for DetailedAnswer
/// </summary>
public class DetailedAnswer : UserAnswer
{
    /// <summary>
    /// DetailedAnswer content 
    /// </summary>
    public string? AnswerContent { get; set; }
    /// <summary>
    /// Accuracy of answer
    /// </summary>
    public int? Accuracy { get; set; }
    /// <summary>
    /// Files which user attach to answer
    /// </summary>
    public List<string>? Files { get; set; } = new();
    /// <summary>
    /// List of reviewed tests
    /// </summary>
    public List<ReviewedDetailedTests>? ReviewedDetailedTests { get; set; } = new();


}