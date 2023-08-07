using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class EducationInfoCreateDto {
    [Required]
    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialization { get; set; }
    public string? Status { get; set; }
    public DateTime? EndTime { get; set; }
}