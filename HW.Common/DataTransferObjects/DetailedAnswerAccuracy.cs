using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class DetailedAnswerAccuracy {
    [Required]
    [Range(1,5)]
    public int Accuracy { get; set; }
}