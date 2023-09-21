using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;
/// <summary>
/// Entity for Test
/// </summary>
public class Test
{
    /// <summary>
    /// Test's id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Date and time the test was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the test was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the test was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// List of sorted tests
    /// </summary>
    public List<Guid>? OrderedTests { get; set; }
    // /// <summary>
    // /// Previous test (for sorting)
    // /// </summary>
    // public Test? PreviousTest { get; set; }
    // /// <summary>
    // /// Next test (for sorting)
    // /// </summary>
    // public Test? NextTest { get; set; }
    /// <summary>
    /// Chapter identifier
    /// </summary>
    public required Chapter Chapter { get; set; }
    /// <summary>
    /// Test's question
    /// </summary>
    public required string Question { get; set; }
    /// <summary>
    /// Test's files
    /// </summary>
    public List<string>? Files { get; set; } = new();
    /// <summary>
    /// User's answers
    /// </summary>

    public List<UserAnswerTest>? UserAnswerTests { get; set; } = new();
    /// <summary>
    /// Test type
    /// </summary>
    public TestType TestType { get; set; }
}
