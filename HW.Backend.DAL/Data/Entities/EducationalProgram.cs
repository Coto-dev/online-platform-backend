namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for EducationalProgram
/// </summary>
public class EducationalProgram
{
    /// <summary>
    /// EducationalProgram identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// EducationalProgram name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// EducationalProgram Description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// EducationalProgram Price
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// Date and time the EducationalProgram was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the EducationalProgram was editted
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the EducationalProgram was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// List of available modules in this EducationalProgram
    /// </summary>
    public List<ModuleInEducationalProgram> AvailableModules { get; set; }
    /// <summary>
    /// List of students in this EducationalProgram
    /// </summary>
    public List<Student> Students { get; set; }

}
