namespace HW.Common.DataTransferObjects; 

public class UserAnswerFullDto {
    public List<Guid>? UserAnswerSimples { get; set; } = new();
    public List<UserAnswerCorrectSequenceDto>? UserAnswerCorrectSequences { get; set; } = new();
    public string? DetailedAnswer { get; set; }
    public bool IsAnswered { get; set; } = false;
}