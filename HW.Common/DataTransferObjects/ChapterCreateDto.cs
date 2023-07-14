using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ChapterCreateDto {
    [Required]
    [MinLength(3)]
    [MaxLength(128)]
    [DefaultValue("Цели моделирования")]
    public string Name { get; set; }
    public string? Content { get; set; } // html
    public List<Guid>? FileIds { get; set; } = new();
}