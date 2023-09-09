using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ModuleStreamingCreateDto {
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarId { get; set; }
    public int? Price { get; set; }
    public List<Guid>? RequiredModules { get; set; } = new();
    public DateTime? ExpirationTime { get; set; }
    public DateTime? StartTime { get; set; }
    public int? MaxStudents { get; set; }
    public List<Guid>? Teachers { get; set; } = new();
    public List<Guid>? Editors { get; set; } = new();
    public DateTime? StopRegistrationDate { get; set; }
    public DateTime? StartRegistrationDate { get; set; }
    public string? TimeDuration { get; set; }
}