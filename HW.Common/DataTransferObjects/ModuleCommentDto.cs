using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ModuleCommentDto {
    [Required]
    public string Message { get; set; }
}