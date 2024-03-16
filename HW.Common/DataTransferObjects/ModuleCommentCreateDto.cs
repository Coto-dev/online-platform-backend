using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ModuleCommentCreateDto {
    [Required]
    public string Message { get; set; }
}