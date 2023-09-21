namespace HW.Backend.DAL.Data.Entities;

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
    public required string Name { get; set; }
    /// <summary>
    /// EducationalProgram Description
    /// </summary>
    public required string Description { get; set; }
    /// <summary>
    /// EducationalProgram Price
    /// </summary>
    public required int Price { get; set; }
    /// <summary>
    /// Module's avatar
    /// </summary>
    public string? AvatarId { get; set; }
    /// <summary>
    /// Date when program starts
    /// </summary>
    public DateTime? StartAt { get; set; }
    /// <summary>
    /// Date when user can register
    /// </summary>
    public DateTime? StartRegisterAt { get; set; }
    /// <summary>
    /// Required time to finish program
    /// </summary>
    public string? TimeDuration { get; set; }
    /// <summary>
    /// Date and time the EducationalProgram was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the EducationalProgram was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the EducationalProgram was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// List of available modules in this EducationalProgram
    /// </summary>
    public List<Module>? Modules { get; set; } = new();
    /// <summary>
    /// List of students on educational program
    /// </summary>
    public List<StudentEducationalProgram>? UserPrograms { get; set; } = new();
    
    /// <summary>
    /// Module author
    /// </summary>
    public required Teacher Author { get; set; }
    
    /// <summary>
    /// List of Editors in this EducationalProgram
    /// </summary>
    public List<Teacher>? Editors { get; set; } = new();

}
