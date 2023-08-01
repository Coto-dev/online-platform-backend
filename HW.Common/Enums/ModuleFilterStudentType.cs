namespace HW.Common.Enums; 

public enum ModuleFilterStudentType {
    /// <summary>
    /// Приобретен
    /// </summary>
    Purchased,
    /// <summary>
    /// В корзине
    /// </summary>
    InCart,
    /// <summary>
    /// Купил/Выполняю
    /// </summary>
    InProcess,
    /// <summary>
    /// Пройденный
    /// </summary>
    Passed,
    /// <summary>
    /// Не пройденный (не успел по времени)
    /// </summary>
    NotPassed
}