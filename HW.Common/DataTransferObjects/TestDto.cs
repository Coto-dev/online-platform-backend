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

    public List<string>? FileUrls { get; set; } = new();
    public List<PossibleAnswerDto> PossibleAnswers { get; set; } = new();
    /// <summary>
    /// если у пользователя не было ответов на тест то пустой список
    /// </summary>
    public UserAnswerFullDto? UserAnswer { get; set; }
    public TestType Type { get; set; }
}