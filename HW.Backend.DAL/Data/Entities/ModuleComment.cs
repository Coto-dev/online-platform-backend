﻿namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for ModuleComment
/// </summary>
public class ModuleComment
{
    /// <summary>
    /// ModuleComment's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// DataTime when was created
    /// </summary>
    public DataTime CreatedAt { get; set; }
    /// <summary>
    /// DataTime when was editted
    /// </summary>
    public DataTime? EdittedAt { get; set; }
    /// <summary>
    /// Comment message
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Module identifier
    /// </summary>
    public Module Module { get; set; }
    /// <summary>
    /// Userbackend identifier
    /// </summary>
    public UserBackend User { get; set; }
    /// <summary>
    /// Bool check is teacher comment
    /// </summary>
    public bool IsTeacherComment { get; set; }

}
