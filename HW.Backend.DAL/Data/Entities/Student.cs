namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for student
/// </summary>
public class Student : UserBackend {
    /// <summary>
    /// List of student's module
    /// </summary>
    public List<StudentModule>? Modules { get; set; } = new();
    /// <summary>
    /// List of student's learned chapters
    /// </summary>
    public List<Chapter>? LearnedChapters { get; set; } = new();
    /// <summary>
    /// List of student's comments to modules
    /// </summary>
    public List<ModuleComment>? ModuleComments { get; set; } = new();
    /// <summary>
    /// User's chapter comments
    /// </summary>
    public List<ChapterComment>? ChapterComments { get; set; } = new();
    public List<EducationalProgram>? EducationalPrograms { get; set; } = new();


}
