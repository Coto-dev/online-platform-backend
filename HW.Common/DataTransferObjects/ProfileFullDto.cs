namespace HW.Common.DataTransferObjects; 

/// <summary>
/// user profile DTO
/// </summary>
public class ProfileFullDto {
    /// <summary>
    /// Profile Identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public required string FullName { get; set; }
    
    /// <summary>
    /// User's work experience
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
    /// Date when user joined the system
    /// </summary>
    public DateTime JoinedAt { get; set; }
    
    /// <summary>
    /// Photo Identifier
    /// </summary>
    public Guid? PhotoId { get; set; }
}