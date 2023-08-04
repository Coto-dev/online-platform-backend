namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for StreamingModule
/// </summary>
public class StreamingModule : Module
{
	/// <summary>
	/// DateTime of module start
	/// </summary>
	public DateTime StartAt { get; set; }
	/// <summary>
	/// Date time when module expired
	/// </summary>
	public DateTime? ExpiredAt { get; set; }
	/// <summary>
	/// Number of maximum students in module
	/// </summary>
	public int MaxStudents { get; set; }

}
