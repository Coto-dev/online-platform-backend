
using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;
/// <summary>
/// Entity for UserAnswerTest
/// </summary>
public class UserAnswerTest
{
    /// <summary>
    /// UserAnswerTest's id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Test identifier
    /// </summary>
    public required Test Test { get; set; }

    /// <summary>
    /// List of UserAnswer for test
    /// </summary>
    public required List<UserAnswer> UserAnswers { get; set; } = new();
    /// <summary>
    /// Number of attempt for test
    /// </summary>
    public required int NumberOfAttempt { get; set; }
    /// <summary>
    /// Datetime check resolve
    /// </summary>
    public DateTime? AnsweredAt { get; set; }
    /// <summary>
    /// User identifier
    /// </summary>
    public required Student Student { get; set; }
    /// <summary>
    /// User answer status
    /// </summary>
    public UserAnswerTestStatus Status { get; set; } = UserAnswerTestStatus.NotDone;

}
