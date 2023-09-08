using System.ComponentModel.DataAnnotations;

namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for StreamingModule
/// </summary>
public class StreamingModule : Module
{
	
	/// <summary>
	/// Datetime when user can register
	/// </summary>
	public DateTime? StartRegisterAt { get; set; }
	/// <summary>
	/// Datetime when user can't register
	/// </summary>
	public DateTime? StopRegisterAt { get; set; }
	/// <summary>
	/// DateTime of module start
	/// </summary>
	public DateTime? StartAt { get; set; }
	/// <summary>
	/// Datetime when module expired
	/// </summary>
	public DateTime? ExpiredAt { get; set; }

	/// <summary>
	/// Number of maximum students in module
	/// </summary>
	public int MaxStudents { get; set; }

}
