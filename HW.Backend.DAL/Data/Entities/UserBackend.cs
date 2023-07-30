namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// UserBackend entity 
/// </summary>
public class UserBackend {
    /// <summary>
    /// UserBackend identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Student? Student { get; set; }
    public Teacher? Teacher { get; set; }
    /// <summary>
    /// User's module comments
    /// </summary>
    public List<ModuleComment>? ModuleComments { get; set; } = new();
    /// <summary>
    /// User's chapter comments
    /// </summary>
    public List<ChapterComment>? ChapterComments { get; set; } = new();

}
