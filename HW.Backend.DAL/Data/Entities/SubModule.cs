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
    /// Date and time the sub module was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the sub module was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the sub module was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// Submodule's name
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Number of order for sorting
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Module 
    /// </summary>
    public required Module Module { get; set; }

    /// <summary>
    /// Sub module type
    /// </summary>
    public SubModuleType SubModuleType { get; set; }

    /// <summary>
    /// List of chapters in submodule
    /// </summary>
    public List<Chapter>? Chapters { get; set; } = new();

}
