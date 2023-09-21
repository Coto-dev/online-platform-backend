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
    /// List of modules where teacher is author
    /// </summary>
    public List<Module> CreatedModules { get; set; }
    /// <summary>
    /// List of modules where teacher is teacher :)
    /// </summary>
    public List<Module>? ControlledModules { get; set; } = new();
    /// <summary>
    /// List of modules where teacher is editor
    /// </summary>
    public List<Module>? EditorModules { get; set; } = new();
    /// <summary>
    /// List of programs where teacher is editor
    /// </summary>
    public List<EducationalProgram>? EditorPrograms { get; set; } = new();
    /// <summary>
    /// List of programs where teacher is author
    /// </summary>
    public List<EducationalProgram>? CreatedPrograms { get; set; } = new();



}
