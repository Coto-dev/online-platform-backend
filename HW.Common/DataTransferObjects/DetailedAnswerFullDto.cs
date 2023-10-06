namespace HW.Common.DataTransferObjects; 

public class DetailedAnswerFullDto {
    public string? AnswerContent { get; set; }
    public int? Accuracy { get; set; }
    public List<FileLinkDto> Files { get; set; } = new();
}