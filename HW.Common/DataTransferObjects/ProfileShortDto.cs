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
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }
    public string? NickName { get; set; }
 
}