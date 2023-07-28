namespace HW.Common.DataTransferObjects; 

public class UserAnswerFullDto {
    public List<UserAnswerSimpleDto>? UserAnswerSimples { get; set; } = new();
    public List<UserAnswerCorrectSequenceDto>? UserAnswerCorrectSequences { get; set; } = new();
    public string? DetailedAnswer { get; set; }
    public int NumberOfAttempt { get; set; } = 0;
}