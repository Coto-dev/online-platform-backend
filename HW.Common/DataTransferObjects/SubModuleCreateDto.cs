using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class SubModuleCreateDto {
    [Required]
    public string Name { get; set; }
}