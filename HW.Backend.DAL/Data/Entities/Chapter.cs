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
    /// Chapter's files
    /// </summary>
    public List<Guid> Files { get; set; }
	/// <summary>
	/// ... 
	/// </summary>
	//public List<Test> TestChapter { get; set; } --------------------------2
	/// <summary>
	/// ...
	/// </summary>
    public string Content { get; set; }



}
