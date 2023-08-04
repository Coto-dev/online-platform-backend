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
    /// List of teacher's controlled modules
    /// </summary>
    public List<Module>? ControlledModules { get; set; } = new();
    /// <summary>
    /// List of teacher's created modules
    /// </summary>
    public List<Module>? CreatedModules { get; set; } = new();


}
