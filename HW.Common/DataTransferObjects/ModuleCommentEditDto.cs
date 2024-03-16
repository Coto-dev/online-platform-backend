using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects;

public class ModuleCommentEditDto
{
    [Required]
    public string Message { get; set; }
}
