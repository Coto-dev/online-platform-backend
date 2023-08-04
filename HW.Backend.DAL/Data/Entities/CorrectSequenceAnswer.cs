namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for CorrectSequenceAnswer
/// </summary>
public class CorrectSequenceAnswer
{
    /// <summary>
    /// CorrectSequenceAnswer identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Answer content
    /// </summary>
    public required string AnswerContent { get; set; }
    /// <summary>
    /// Right order of answer
    /// </summary>
    public required int RightOrder { get; set; }
    /// <summary>
    /// CorrectSequenceTest identifier
    /// </summary>
    public required CorrectSequenceTest CorrectSequenceTest { get; set; }

}