﻿using HW.Backend.Dal.Data.Entities;
using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for module
/// </summary>
public class Module
{ 
    /// <summary>
    /// Module's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Module's name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Module's description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Module's price
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// Date and time the module was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the module was editted
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the module was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// Module's type
    /// </summary>
    public DateTime ModuleType { get; set; }
    /// <summary>
    /// Teacher-creator identifier
    /// </summary>
    public Teacher Creator { get; set; }
    /// <summary>
    /// List of students on module
    /// </summary>
    public List<Student> Students { get; set; }
    /// <summary>
    /// List of submodules 
    /// </summary>
    public List<SubModule> SubModules { get; set; }
    /// <summary>
    /// Module's visibility
    /// </summary>
    public ModuleVisibilityType ModuleVisibility { get; set; }

}
