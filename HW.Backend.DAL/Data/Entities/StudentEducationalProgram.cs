using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities; 

public class StudentEducationalProgram {
    /// <summary>
    /// Program identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Program
    /// </summary>
    public required EducationalProgram EducationalProgram { get; set; }
    /// <summary>
    /// Student
    /// </summary>
    public required Student Student { get; set; }
    /// <summary>
    /// Relationship between program and student
    /// </summary>
    public ProgramVisibilityType ProgramVisibilityType { get; set; } 
}