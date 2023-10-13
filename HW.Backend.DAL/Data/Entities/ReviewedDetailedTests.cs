namespace HW.Backend.DAL.Data.Entities; 

public class ReviewedDetailedTests {
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Detailed test
    /// </summary>
    public required DetailedAnswer DetailedAnswer { get; set; }
    /// <summary>
    /// Teacher who set the accuracy test
    /// </summary>
    public required Teacher ReviewedBy { get; set; }
    /// <summary>
    /// Time when Teacher set the accuracy
    /// </summary>
    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
}