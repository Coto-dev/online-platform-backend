namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for CorrectSequenceTest
/// </summary>
public class CorrectSequenceTest : Test {
    /// <summary>
    /// List of possible answers for correct sequence test
    /// </summary>
    public required List<CorrectSequenceAnswer> PossibleAnswers { get; set; } = new();
}