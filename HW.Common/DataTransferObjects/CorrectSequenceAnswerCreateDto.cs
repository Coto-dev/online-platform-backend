using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 
/// <summary>
/// вариант ответа для другого типа теста
/// </summary>
public class CorrectSequenceAnswerCreateDto {
    [Required]
    public string AnswerContent { get; set; }
   
}