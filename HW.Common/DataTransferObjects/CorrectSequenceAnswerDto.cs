namespace HW.Common.DataTransferObjects; 

public class CorrectSequenceAnswerDto {
    public Guid Id { get; set; }
    public string AnswerContent { get; set; }
    public int RightOrder { get; set; }
}