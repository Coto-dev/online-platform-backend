using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

/// <summary>
/// Profile DTO for Edit
/// </summary>
public class ProfileEditDto {
    
    
    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    [Required]
    public required string FullName { get; set; }
    
    /// <summary>
    /// User's avatar id 
    /// </summary>
    [Required]
    public Guid PhotoId { get; set; }
    /// <summary>
    /// User's work experience
    /// </summary>
    [Required]
    public string WorkExperience { get; set; }
    
    /// <summary>
    /// User's location(city)
    /// </summary>
    [Required]
    public string Location { get; set; }
    
    /// <summary>
    /// User's education grade
    /// </summary>
    [Required]
    public string Education { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    [Required]
    [Range(typeof(DateTime), "01/01/1900", "01/01/2023")]
    public required DateTime BirthDate { get; set; }

}