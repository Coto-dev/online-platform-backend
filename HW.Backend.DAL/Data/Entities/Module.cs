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
    public required string Name { get; set; }
    /// <summary>
    /// Module's description
    /// </summary>
    public required string Description { get; set; }
    /// <summary>
    /// Module's price
    /// </summary>
    public required int Price { get; set; }
    /// <summary>
    /// Module's avatar
    /// </summary>
    public string? AvatarId { get; set; }
    /// <summary>
    /// Required time to finish program
    /// </summary>
    public string? TimeDuration { get; set; }
    /// <summary>
    /// Date and time the module was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the module was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the module was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// List of students on module
    /// </summary>
    public List<StudentModule>? UserModules { get; set; } = new();

    /// <summary>
    /// List of submodules 
    /// </summary>
    public List<SubModule>? SubModules { get; set; } = new();
    /// <summary>
    /// Module's visibility
    /// </summary>
    public required ModuleVisibilityType ModuleVisibility { get; set; }
    
     /// <summary>
     /// Module author
     /// </summary>
    public required Teacher Author { get; set; }

    /// <summary>
    /// The target audience of module
    /// </summary>
    public string? TargetAudience { get; set; }

    /// <summary>
    /// Field describing what you will learn
    /// </summary>
    public string? WhatWillYouLearn { get; set; }

    /// <summary>
    /// List of teachers who teach this module
    /// </summary>
    public List<Teacher>? Teachers { get; set; } = new();

    /// <summary>
    /// List of sorted sub Modules
    /// </summary>
    public List<Guid>? OrderedSubModules { get; set; } = new();
    /// <summary>
    /// List of creators
    /// </summary>
    public List<Teacher>? Editors { get; set; } = new();
    /// <summary>
    /// List of educational programs
    /// </summary>
    public List<EducationalProgram>? Programs { get; set; } = new();
    /// <summary>
    /// List of recommended modules 
    /// </summary>
    public List<Module>? RecommendedModules { get; set; } = new();
    /// <summary>
    /// List of tags
    /// </summary>
    public List<ModuleTag>? Tags { get; set; } = new();

}
