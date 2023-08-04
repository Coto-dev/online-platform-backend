namespace HW.Backend.DAL.Data.Entities;

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
    public required DateTime CreatedAt { get; set; }
    /// <summary>
    /// DataTime when was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Comment message
    /// </summary>
    public required string Message { get; set; }
    /// <summary>
    /// Module identifier
    /// </summary>
    public required Module Module { get; set; }
    /// <summary>
    /// UserBackend identifier
    /// </summary>
    public required UserBackend User { get; set; }
    /// <summary>
    /// Bool check is teacher comment
    /// </summary>
    public required bool IsTeacherComment { get; set; }

}
