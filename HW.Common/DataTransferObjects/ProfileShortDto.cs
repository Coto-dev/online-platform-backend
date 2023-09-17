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
    
    /// <summary>
    /// Myself description
    /// </summary>
    public string? AboutMe { get; set; }
    
    /// <summary>
    /// User's post
    /// </summary>
    public string? Post { get; set; }
 
}