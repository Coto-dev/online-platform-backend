namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for StreamingModule
/// </summary>
public class StreamingModule : Module
{
	/// <summary>
	/// DataTime of module start
	/// </summary>
	public DataTime StartAt { get; set; }
	/// <summary>
	/// Number of maximum students in module
	/// </summary>
	public int MaxStudents { get; set; }

}
