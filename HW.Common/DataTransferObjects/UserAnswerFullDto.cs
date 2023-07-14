namespace HW.Common.DataTransferObjects; 

public class UserAnswerFullDto {
    public List<UserAnswerSimpleDto>? UserAnswerSimples { get; set; }
    public List<UserAnswerCorrectSequenceDto>? UserAnswerCorrectSequences { get; set; }
}