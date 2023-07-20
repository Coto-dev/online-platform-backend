using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class EducationInfoCreateDto {
    [Required]
    public string University { get; set; }
    [Required]
    public string Faculty { get; set; }
    [Required]
    public string Specialization { get; set; }
    [Required]
    public string Status { get; set; }
}