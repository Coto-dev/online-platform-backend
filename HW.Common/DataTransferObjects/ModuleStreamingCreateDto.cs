using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ModuleStreamingCreateDto {
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? Price { get; set; }
    public List<RequiredModulesDto>? RequiredModules { get; set; } = new();
    public DateTime? ExpirationTime { get; set; }
    public DateTime? StartAt { get; set; }
    public int? MaxStudents { get; set; }
    public List<Guid>? Teachers { get; set; } = new();
    public string? TimeDuration { get; set; }
}