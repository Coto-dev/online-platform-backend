using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 
/// <summary>
/// Моделька, чтобы преподаватель создавал тест
/// </summary>
public class TestSimpleCreateDto {
    [Required]
    public string Question { get; set; }
    [Required]
    public List<string>? FileIds { get; set; }
}