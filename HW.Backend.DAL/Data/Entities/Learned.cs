namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for learned chapter by user
/// </summary>
public class Learned
{
    /// <summary>
    /// Learned's id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// User identifier
    /// </summary>
    public UserBackend LearnedBy { get; set; }
    /// <summary>
    /// Chapter identifier
    /// </summary>
    public Chapter Chapter { get; set; }
    /// <summary>
    /// DataTime when was learned
    /// </summary>
    public DataTime LearnDate { get; set; }

}
