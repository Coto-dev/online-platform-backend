using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 
/// <summary>
/// Вариант ответа для простого типа теста
/// </summary>
public class SimpleAnswerCreateDto {
    
    [Required]
    public string AnswerContent { get; set; }
    [Required]
    public bool isRight { get; set; }
}