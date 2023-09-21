using Microsoft.AspNetCore.Identity;

namespace HW.Account.DAL.Data.Entities; 

/// <summary>
/// AuthDB general User Model   
/// </summary>
public class User : IdentityUser<Guid> {
    /// <summary>
    /// User`s full name
    /// </summary>
    public  string? FullName { get; set; }
    /// <summary>
    /// User's nickname
    /// </summary>
    public required string NickName { get; set; }

    /// <summary>
    /// User's work experience
    /// </summary>
    public WorkExperience WorkExperience { get; set; }

    /// <summary>
    /// User's location(city)
    /// </summary>
    public Location Location { get; set; } 

    /// <summary>
    /// User's education grade
    /// </summary>
    public Education Education { get; set; } 

    /// <summary>
    /// User`s birth date
    /// </summary>
    public BirthDate BirthDate { get; set; } 

    /// <summary>
    /// Avatar id
    /// </summary>
    public string? AvatarId { get; set; }
    /// <summary>
    /// Myself description
    /// </summary>
    public string? AboutMe { get; set; }
    /// <summary>
    /// User's post
    /// </summary>
    public string? Post { get; set; }
    /// <summary>
    /// Date when user joined the system
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User`s devices
    /// </summary>
    public List<Device> Devices { get; set; } = new List<Device>();
}