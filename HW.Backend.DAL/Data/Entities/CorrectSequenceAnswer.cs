namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for CorrectSequenceAnswer
/// </summary>
public class CorrectSequenceAnswer
{
    /// <summary>
    /// CorrectSequenceAnswer identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Answer content
    /// </summary>
    public required string AnswerContent { get; set; }
    /// <summary>
    /// Right order of answer
    /// </summary>
    public required int RightOrder { get; set; }
    /// <summary>
    /// CorrectSequenceTest identifier
    /// </summary>
    public required CorrectSequenceTest CorrectSequenceTest { get; set; }
    /// <summary>
    /// Date and time the answer was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the answer was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the answer was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

}