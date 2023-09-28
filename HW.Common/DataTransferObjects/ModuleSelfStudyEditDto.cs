using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleSelfStudyEditDto {
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    public string? AvatarId { get; set; }
    [Required]
    public int Price { get; set; }
    [Required]
    public List<Guid>? RequiredModules { get; set; } = new();
    [Required]
    public string? TimeDuration { get; set; }
}