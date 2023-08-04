using Microsoft.AspNetCore.Identity;

namespace HW.Account.DAL.Data.Entities; 

/// <summary>
/// AuthDB general User Model   
/// </summary>
public class User : IdentityUser<Guid> {
    /// <summary>
    /// User`s full name
    /// </summary>
    public required string FullName { get; set; }
    /// <summary>
    /// User's nickname
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// User's work experience
    /// </summary>
    public required WorkExperience WorkExperience { get; set; } = new WorkExperience();

    /// <summary>
    /// User's location(city)
    /// </summary>
    public required Location Location { get; set; } = new Location();

    /// <summary>
    /// User's education grade
    /// </summary>
    public required Education Education { get; set; } = new Education();

    /// <summary>
    /// User`s birth date
    /// </summary>
    public required BirthDate BirthDate { get; set; } = new BirthDate();

    /// <summary>
    /// Avatar id
    /// </summary>
    public string? AvatarId { get; set; }

    /// <summary>
    /// Date when user joined the system
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User`s devices
    /// </summary>
    public List<Device> Devices { get; set; } = new List<Device>();
}