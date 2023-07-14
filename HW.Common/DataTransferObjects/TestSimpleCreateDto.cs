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
    public TestSimpleType TestType { get; set; }
    [Required]
    public List<SimpleAnswerDto> PossibleAnswers { get; set; }
    public List<Guid>? FileIds { get; set; }
}