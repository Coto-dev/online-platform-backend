namespace HW.Common.DataTransferObjects; 

/// <summary>
/// Short profile DTO
/// </summary>
public class ProfileShortDto {
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }
 
    /// <summary>
    /// Photo Identifier
    /// </summary>
    public string? AvatarId { get; set; }
 
    /// <summary>
    /// User`s nickname
    /// </summary>
    public string? NickName { get; set; }
 
}