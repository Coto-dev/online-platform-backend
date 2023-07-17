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
    /// List of student's learned chapters
    /// </summary>
    public List<Chapter> LearnedChapters { get; set; }
    /// <summary>
    /// 
    /// </summary>

}
