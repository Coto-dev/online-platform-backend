using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ChapterCreateDto {
    [Required]
    [MinLength(3)]
    [MaxLength(128)]
    [DefaultValue("Цели моделирования")]
    public string Name { get; set; }

    [Required]
    [DefaultValue(Enums.ChapterType.DefaultChapter)]
    public ChapterType ChapterType { get; set; } = ChapterType.DefaultChapter;
    public string? Content { get; set; } // html
    public List<string>? FileIds { get; set; } = new();
}