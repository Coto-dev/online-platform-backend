using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleSelfStudyCreateDto {
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? Price { get; set; }
    public List<RequiredModulesDto>? RequiredModules { get; set; } = new();
    public string? TimeDuration { get; set; }
    public List<Guid>? Teachers { get; set; } = new();

}