using System.ComponentModel;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class TestDto {
    /// <summary>
    /// Если потребуется реализовать новые тесты, добавить сюда поля
    /// </summary>
    public Guid Id { get; set; }
    [DefaultValue("Где рождаются волки?🤔")]
    public string Question { get; set; }
    public List<FileLinkDto>? FileIds { get; set; }
    public List<PossibleAnswerDto> PossibleAnswers { get; set; }
    /// <summary>
    /// если у пользователя не было ответов на тест то null
    /// </summary>
    public UserAnswerFullDto? UserAnswer { get; set; }
    public TestType Type { get; set; }
}