namespace HW.Common.Enums; 

public enum TestType{
    /// <summary>
    /// Один вариант ответа правильный
    /// </summary>
    SingleAnswer,
    /// <summary>
    /// Несколько правильных вариантов
    /// </summary>
    MultipleAnswer,
    /// <summary>
    /// Исключи один лишний
    /// </summary>
    ExtraAnswer,
    /// <summary>
    /// Исключи несколько лишних
    /// </summary>
    MultipleExtraAnswer,
    /// <summary>
    /// Правильная последовательность ответов
    /// </summary>
    CorrectSequenceAnswer
}