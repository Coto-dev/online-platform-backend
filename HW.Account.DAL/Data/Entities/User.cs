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
    /// User;s work experience
    /// </summary>
    public string? WorkExperience { get; set; }
    /// <summary>
    /// User's location(city)
    /// </summary>
    public string? Location { get; set; }
    /// <summary>
    /// User's education grade
    /// </summary>
    public string? Education { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    public DateTime BirthDate { get; set; }
    /// <summary>
    /// Photo id
    /// </summary>
    public Guid? PhotoId { get; set; }

    /// <summary>
    /// Date when user joined the system
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User`s devices
    /// </summary>
    public List<Device> Devices { get; set; } = new List<Device>();
}