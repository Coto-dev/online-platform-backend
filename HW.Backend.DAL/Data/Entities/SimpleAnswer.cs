namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for chapter
/// </summary>
public class SimpleAnswer
{
    /// <summary>
    /// Simple answer identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Answer content
    /// </summary>
    public required string AnswerContent { get; set; }
    /// <summary>
    /// Checker ------------------------------------!
    /// </summary>
    public required bool IsRight { get; set; }
    /// <summary>
    /// SimpleAnswerTest identifier
    /// </summary>
    public required SimpleAnswerTest SimpleAnswerTest { get; set; }
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
