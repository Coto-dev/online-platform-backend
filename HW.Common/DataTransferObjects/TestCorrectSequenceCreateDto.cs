using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class TestCorrectSequenceCreateDto {
    [Required]
    public string Question { get; set; }
    [Required]
    public List<CorrectSequenceAnswerDto> PossibleAnswers { get; set; }
    public List<Guid>? FileIds { get; set; }

}