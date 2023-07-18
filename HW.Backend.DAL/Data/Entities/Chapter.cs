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
    public string Name { get; set; }
    /// <summary>
    /// Chapter's content
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// Submodule in which the chapter
    /// </summary>
    public SubModule SubModule { get; set; }
    /// <summary>
    /// Chapter's files
    /// </summary>
    public List<Guid> Files { get; set; }
	/// <summary>
	/// Chapter's tests
	/// </summary>
	public List<Test> TestChapter { get; set; }

}
