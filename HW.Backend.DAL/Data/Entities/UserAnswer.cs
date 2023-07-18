namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for UserAnswer
/// </summary>
public class UserAnswer
{
    /// <summary>
    /// UserAnswer identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// UserAnswerTest identifier
    /// </summary>
    public required UserAnswerTest UserAnswerTest { get; set; }

}