using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleStreamingEditDto {
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    [Required]
    [MinLength(6)]
    [MaxLength(450)]
    public string Description { get; set; }
    [Required]
    public int Price { get; set; }
    [Required]
    public string? AvatarId { get; set; }
    [Required]
    public List<RequiredModulesDto>? RequiredModules { get; set; } = new();
    [Required]
    public string? TimeDuration { get; set; }
    [Required]
    public ModuleVisibilityType VisibilityType { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime ExpirationTime { get; set; }
    [Required]
    public int MaxStudents { get; set; }
    [Required]
    public List<Guid> Teachers { get; set; } = new();
    public List<Guid>? Creators { get; set; } = new();
}