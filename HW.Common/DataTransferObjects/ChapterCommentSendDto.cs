using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ChapterCommentSendDto {
    [Required]
    public string Message { get; set; }
}