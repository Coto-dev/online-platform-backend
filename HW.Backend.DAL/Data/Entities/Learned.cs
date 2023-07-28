namespace HW.Backend.DAL.Data.Entities;

/// <summary>
/// Entity for learned chapter by user
/// </summary>
public class Learned
{
    /// <summary>
    /// Learned's id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// User identifier
    /// </summary>
    public required UserBackend LearnedBy { get; set; }
    /// <summary>
    /// Chapter identifier
    /// </summary>
    public required Chapter Chapter { get; set; }
    /// <summary>
    /// DataTime when was learned
    /// </summary>
    public DateTime? LearnDate { get; set; }

}
