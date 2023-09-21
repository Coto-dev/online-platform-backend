using System.ComponentModel.DataAnnotations.Schema;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for student
/// </summary>
public class Student {
    /// <summary>
    /// Student id = User id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Link to main user
    /// </summary>
    public required UserBackend UserBackend { get; set; }
    /// <summary>
    /// List of student's module
    /// </summary>
    public List<StudentModule>? Modules { get; set; } = new();
    /// <summary>
    /// List of student's learned chapters
    /// </summary>
    public List<Chapter>? LearnedChapters { get; set; } = new();
    /// <summary>
    /// User educational programs
    /// </summary>
    public List<StudentEducationalProgram>? EducationalPrograms { get; set; } = new();


}
