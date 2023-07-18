namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for UserActions
/// </summary>
public class UserAction
{
    /// <summary>
    /// UserActions identifier
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Student identifier
    /// </summary>
    public required Student Student { get; set; }
    /// <summary>
    /// Count of user's actions
    /// </summary>
    public int Actions { get; set; }

}
