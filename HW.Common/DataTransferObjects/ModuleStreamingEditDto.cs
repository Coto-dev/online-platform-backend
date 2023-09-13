using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ModuleStreamingEditDto {
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    [Required]
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
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationTime { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StopRegistrationDate { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartRegistrationDate { get; set; }
    [Required]
    public int MaxStudents { get; set; }
    [Required]
    public List<Guid> Teachers { get; set; } = new();
    public List<Guid>? Editors { get; set; } = new();
}