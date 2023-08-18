using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects;
/// <summary>
/// Ответ на detailed тест
/// </summary>
public class DetailedAnswerDto
{
    public string AnswerContent { get; set; }
    public List<string> Files { get; set; }
}