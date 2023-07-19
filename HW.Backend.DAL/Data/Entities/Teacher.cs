namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for teacher
/// </summary>
public class Teacher : UserBackend
{
    /// <summary>
    /// List of teacher's controlled modules
    /// </summary>
    public List<Module>? ControlledModules { get; set; }

}
