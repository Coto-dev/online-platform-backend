using HW.Backend.DAL.Data.Entities;

namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for teacher
/// </summary>
public class Teacher
{
    /// <summary>
    /// Teacher identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// List of teacher's contrelled modules
    /// </summary>
    public List<Module>? ControlledModules { get; set; }

}
