namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for UserModule
/// </summary>
public class UserModule
{
    /// <summary>
    /// UserModule identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Student identifier
    /// </summary>
    public Student Stundet { get; set; }
    /// <summary>
    /// List of stundent modules
    /// </summary>
    public List<Module> Modules { get; set; }

}
