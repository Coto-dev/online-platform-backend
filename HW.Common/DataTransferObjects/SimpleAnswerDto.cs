namespace HW.Common.DataTransferObjects; 

public class SimpleAnswerDto {
    public Guid Id { get; set; }
    public string AnswerContent { get; set; }
    public bool isRight { get; set; }
}