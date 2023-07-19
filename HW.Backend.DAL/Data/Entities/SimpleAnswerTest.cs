namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for SimpleAnswerTest
/// </summary>
public class SimpleAnswerTest : Test
{
    /// <summary>
    /// List of possible answers for simple test
    /// </summary>
    public required List<SimpleAnswer> PossibleAnswers { get; set; }

}