using System.ComponentModel.DataAnnotations.Schema;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for teacher
/// </summary>
public class Teacher {
    /// <summary>
    /// Teacher id = User id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Link to main user
    /// </summary>
    public required UserBackend UserBackend { get; set; }
    /// <summary>
    /// List of modules where teacher is teacher :)
    /// </summary>
    public List<Module>? ControlledModules { get; set; } = new();
    /// <summary>
    /// List of modules where teacher is creator
    /// </summary>
    public List<Module>? CreatedModules { get; set; } = new();
    /// <summary>
    /// List of programs where teacher is creator
    /// </summary>
    public List<EducationalProgram>? CreatedPrograms { get; set; } = new();


}
