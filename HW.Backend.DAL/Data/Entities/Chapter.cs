using HW.Common.Enums;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for chapter
/// </summary>
public class Chapter
{
    /// <summary>
    /// Chapter id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Date and time the chapter was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Date and time the chapter was edited
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Date and time the chapter was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
    /// <summary>
    /// List of sorted tests
    /// </summary>
    public List<Guid>? OrderedTests { get; set; } = new();
    /// <summary>
    /// Chapter's name
    /// </summary>
    public required string Name { get; set; }
    // /// <summary>
    // /// Previous chapter (for sorting)
    // /// </summary>
    // public Chapter? PreviousChapter { get; set; }
    // /// <summary>
    // /// Next chapter (for sorting)
    // /// </summary>
    // public Chapter? NextChapter { get; set; }
    /// <summary>
    /// Chapter's content
    /// </summary>
    public string? Content { get; set; }
    /// <summary>
    /// Submodule in which the chapter
    /// </summary>
    public required SubModule SubModule { get; set; }

    /// <summary>
    /// Chapter's files
    /// </summary>
    public List<string>? Files { get; set; } = new();

    /// <summary>
    /// Chapter's tests
    /// </summary>
    public List<Test>? ChapterTests { get; set; } = new();
    /// <summary>
    /// Chapter type
    /// </summary>
	public ChapterType ChapterType { get; set; }

    /// <summary>
    /// Chapter comments
    /// </summary>

    public List<ChapterComment>? ChapterComments { get; set; } = new();
}
