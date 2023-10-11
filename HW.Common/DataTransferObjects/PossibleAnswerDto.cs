namespace HW.Common.DataTransferObjects; 
/// <summary>
/// возможный вариант ответа для студента
/// </summary>
public class PossibleAnswerDto {
    public Guid Id { get; set; }
    public string AnswerContent { get; set; }
    public bool? IsRight { get; set; }
    public int? RightOrder { get; set; }
}