using System.ComponentModel;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class TestTeacherDto {
    /// <summary>
    /// Если потребуется реализовать новые тесты, добавить сюда поля
    /// </summary>
    public Guid Id { get; set; }
    public string Question { get; set; } = "";
    public List<FileLinkDto>? FileIds { get; set; } = new();
    public List<SimpleAnswerDto>? PossibleSimpleAnswers { get; set; } = new();
    public List<CorrectSequenceAnswerDto>? PossibleCorrectSequenceAnswers { get; set; } = new();
    /// <summary>
    /// Это будет что-то типо ключевых слов
    /// </summary>
    public string? KeyWords { get; set; }
    public TestType Type { get; set; }
}