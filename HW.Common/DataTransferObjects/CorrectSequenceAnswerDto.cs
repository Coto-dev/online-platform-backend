using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 
/// <summary>
/// вариант ответа для другого типа теста
/// </summary>
public class CorrectSequenceAnswerDto {
    [Required]
    public string AnswerContent { get; set; }
    /// <summary>
    /// 0 - если пользователь не расположил ответ нигде
    /// </summary>
    [Required]
    [Range(0,15)]
    public int RightOrder { get; set; }
}