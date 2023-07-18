using HW.Backend.Dal.Data.Entities;

namespace HW.Backend.DAL.Data.Entities;
/// <summary>
/// Entity for UserAnswerTest
/// </summary>
public class UserAnswerTest
{
    /// <summary>
    /// UserAnswerTest's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Test identifier
    /// </summary>
    public required Test Test { get; set; }
    /// <summary>
    /// List of UserAnswer for test
    /// </summary>
    public required List<UserAnswer> UserAnswers { get; set; }
    /// <summary>
    /// Number of attempt for test
    /// </summary>
    public required int NumberOfAttempt { get; set; }
    /// <summary>
    /// Bool check resolve
    /// </summary>
    public required bool IsAnswered { get; set; }
    /// <summary>
    /// User identifier
    /// </summary>
    public required Student Student { get; set; }

}
