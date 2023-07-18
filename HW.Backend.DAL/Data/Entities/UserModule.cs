using HW.Backend.DAL.Data.Entities;
using HW.Common.Enums;

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
    public required Student Stundet { get; set; }
    /// <summary>
    /// List of stundent modules
    /// </summary>
    public required Module Module { get; set; }
    /// <summary>
    /// Module status type
    /// </summary>
    public ModuleStatusType ModuleStatus { get; set; }

}
