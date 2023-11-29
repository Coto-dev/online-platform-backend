using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects;


/// <summary>
/// Dto for email
/// </summary>
public class EmailDto
{
    /// <summary>
    /// User email
    /// </summary>
    [Required]
    [EmailAddress]
    [DisplayName("email")]
    public required string Email { get; set; }
}