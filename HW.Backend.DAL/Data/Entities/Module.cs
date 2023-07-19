using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for module
/// </summary>
public class Module
{ 
    /// <summary>
    /// Module's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Module's name
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Module's description
    /// </summary>
    public required string Description { get; set; }
    /// <summary>
    /// Module's price
    /// </summary>
    public required int Price { get; set; }
    /// <summary>
    /// Date and time the module was created
    /// </summary>
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the module was editted
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the module was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// Teacher-creator identifier
    /// </summary>
    public required Teacher Creator { get; set; }
    /// <summary>
    /// List of students on module
    /// </summary>
    public List<Student>? Students { get; set; }
    /// <summary>
    /// List of submodules 
    /// </summary>
    public required List<SubModule> SubModules { get; set; }
    /// <summary>
    /// Module's visibility
    /// </summary>
    public required ModuleVisibilityType ModuleVisibility { get; set; }
    /// <summary>
    /// List of teachers who teach this module
    /// </summary>
    public List<Teacher>? Teachers { get; set; }

}
