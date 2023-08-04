    namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for SimpleAnswer
/// </summary>
public class SimpleUserAnswer : UserAnswer
{
    /// <summary>
    /// SimpleAnswer identifier
    /// </summary>
    public required SimpleAnswer SimpleAnswer { get; set; }
   
   // public required string UserSimpleAnswer { get; set; } //---------------------!

}