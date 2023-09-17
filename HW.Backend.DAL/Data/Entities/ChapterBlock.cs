namespace HW.Backend.DAL.Data.Entities; 

public class ChapterBlock {
    /// <summary>
    /// Block id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Date and time the block was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the block was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the block was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// Block's content
    /// </summary>
    public string? Content { get; set; }
    /// <summary>
    /// Block's files
    /// </summary>
    public List<string>? Files { get; set; } = new();
    /// <summary>
    /// Block in which the block
    /// </summary>
    public required Chapter Chapter { get; set; }
}