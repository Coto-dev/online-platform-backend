using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class SubModuleCreateDto {
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
}