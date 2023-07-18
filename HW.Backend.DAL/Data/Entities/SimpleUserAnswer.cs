using Microsoft.Extensions.Hosting;

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
    /// <summary>
    /// SimpleAnswer identifier
    /// </summary>
    public required string UserSimpleAnswer { get; set; } //---------------------!

}