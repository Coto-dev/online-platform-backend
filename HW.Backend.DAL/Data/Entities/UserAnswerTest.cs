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
    public Test Test { get; set; }
    /// <summary>
    /// List of UserAnswer for test
    /// </summary>
    public List<UserAnswer> UserAnswers { get; set; }
    /// <summary>
    /// Number of attempt for test
    /// </summary>
    public int NumberOfAttempt { get; set; }
    /// <summary>
    /// Bool check resolve
    /// </summary>
    public bool IsAnswered { get; set; }
    /// <summary>
    /// User identifier
    /// </summary>
    public Student Student { get; set; }

}
