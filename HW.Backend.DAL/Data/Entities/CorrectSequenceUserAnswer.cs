namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for CorrectSequenceUserAnswer
/// </summary>
public class CorrectSequenceUserAnswer : UserAnswer
{
    /// <summary>
    /// CorrectSequenceAnswer identifier
    /// </summary>
    public required CorrectSequenceAnswer CorrectSequenceAnswer { get; set; }
    /// <summary>
    /// User's order in answer
    /// </summary>
    public required int Order { get; set; }

}
