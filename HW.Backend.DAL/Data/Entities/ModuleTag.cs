namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for module's tag
/// </summary>
public class ModuleTag
{   
    /// <summary>
    /// Tag's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Tag's name
    /// </summary>
    public string TagName { get; set; }
    /// <summary>
    /// Modules with that tag
    /// </summary>
    public List<Module>? Modules { get; set; }

}
