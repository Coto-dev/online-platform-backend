using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for chapter
/// </summary>
public class SubModule
{
    /// <summary>
    /// Sub module id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Submodule's name
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Sub module type
    /// </summary>
    public SubModuleType SubModuleType { get; set; }

    /// <summary>
    /// List of chapters in submodule
    /// </summary>
    public List<Chapter>? Chapters { get; set; } = new();

}
