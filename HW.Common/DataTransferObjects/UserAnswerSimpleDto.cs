using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class UserAnswerSimpleDto {
    [Required]
    public Guid Id { get; set; }
}
