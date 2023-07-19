namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for chapter
/// </summary>
public class SimpleAnswer
{
    /// <summary>
    /// Simple answer identifier
    /// </summary>
    public Guid Id { get; set; }
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

}
