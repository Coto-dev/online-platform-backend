namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for ChapterComment
/// </summary>
public class ChapterComment
{
    /// <summary>
    /// Chapter comment identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Chapter identifier
    /// </summary>
    public required Chapter Chapter { get; set; }
    /// <summary>
    /// User identifier
    /// </summary>
    public required UserBackend User { get; set; }
    /// <summary>
    /// Checker is teacher comment 
    /// </summary>
    public required bool IsTeacherComment { get; set; }
    /// <summary>
    /// Comment content
    /// </summary>
    public string Comment { get; set; }
}
