namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for chapter
/// </summary>
public class SubModule
{
    /// <summary>
    /// Submodule id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Submodule's name
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// List of chapters in submodule
    /// </summary>
    public required List<Chapter> Chapters { get; set; }

}
