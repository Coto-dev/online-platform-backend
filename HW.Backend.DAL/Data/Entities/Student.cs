using HW.Backend.DAL.Data.Entities;

namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for student
/// </summary>
public class Student
{
    /// <summary>
    /// User identifier
    /// </summary>
    public UserBackend User { get; set; }
    /// <summary>
    /// List of students's module
    /// </summary>
    public List<UserModule> Modules { get; set; }
    /// <summary>
    /// List of student's learned chapters
    /// </summary>
    public List<Chapter> LearnedChapters { get; set; }
    /// <summary>
    /// List of student's comennts to modules
    /// </summary>
    public List<ModuleComment> ModuleComments { get; set; }
    /// <summary>
    /// ...
    /// </summary>
    public List<TestComments> TestComments { get; set; } //---------------------!

}
