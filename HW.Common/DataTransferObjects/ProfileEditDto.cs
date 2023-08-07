using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

/// <summary>
/// Profile DTO for Edit
/// </summary>
public class ProfileEditDto {
    
    
    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }
    
    [Required]
    public required string NickName { get; set; }
    
    /// <summary>
    /// User's avatar id 
    /// </summary>
    public string? AvatarId { get; set; }

    /// <summary>
    /// User's location(city)
    /// </summary>
    public string? Location { get; set; }
    

    /// <summary>
    /// User`s birth date
    /// </summary>
    public  DateTime? BirthDate { get; set; }

}