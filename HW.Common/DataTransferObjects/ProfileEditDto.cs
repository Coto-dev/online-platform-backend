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
    [Required]
    public string? FullName { get; set; }
    
    [Required]
    public required string NickName { get; set; }
    
    /// <summary>
    /// User's avatar id 
    /// </summary>
    [Required]
    public string? AvatarId { get; set; }

    /// <summary>
    /// User's location(city)
    /// </summary>
    [Required]
    public string? Location { get; set; }
    

    /// <summary>
    /// User`s birth date
    /// </summary>
    [Required]
    public  DateTime BirthDate { get; set; }

}