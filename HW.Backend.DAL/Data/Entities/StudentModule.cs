using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for UserModule
/// </summary>
public class StudentModule
{
    /// <summary>
    /// UserModule identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Student identifier
    /// </summary>
    public required Student Student { get; set; }
    /// <summary>
    /// List of students modules
    /// </summary>
    public required Module Module { get; set; }
    /// <summary>
    /// Relationship between module and student
    /// </summary>
    public ModuleStatusType ModuleStatus { get; set; }

}
