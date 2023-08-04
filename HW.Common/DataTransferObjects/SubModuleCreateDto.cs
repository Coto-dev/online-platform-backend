using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class SubModuleCreateDto {
    [Required]
    [MinLength(3)]
    public string Name { get; set; }

    [Required] 
    public SubModuleType SubModuleType { get; set; } = SubModuleType.DefaultSubModule;
}