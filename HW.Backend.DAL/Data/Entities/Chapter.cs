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
    /// Chapter's name
    /// </summary>
    public required string Name { get; set; }
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
}
