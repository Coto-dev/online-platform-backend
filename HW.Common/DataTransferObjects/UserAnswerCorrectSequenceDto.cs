using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 
/// <summary>
/// Ответ пользователя 
/// </summary>
public class UserAnswerCorrectSequenceDto {
    [Required]
    public Guid Id { get; set; }
    [Required]
    public int Order { get; set; }
}