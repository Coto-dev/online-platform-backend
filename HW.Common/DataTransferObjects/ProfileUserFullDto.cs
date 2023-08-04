namespace HW.Common.DataTransferObjects; 

public class ProfileUserFullDto {
    /// <summary>
    /// Profile Identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }
    /// <summary>
    /// User's nickname
    /// </summary>
    public required string NickName { get; set; }

    /// <summary>
    /// User's work experience
    /// </summary>
    public List<WorkExperienceInfoDto> WorkExperienceInfos { get; set; } = new();
    
    /// <summary>
    /// User's location(city)
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// User's education grade
    /// </summary>
    public List<EducationInfoDto>? EducationInfos { get; set; } = new();

    /// <summary>
    /// User`s birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Photo Identifier
    /// </summary>
    public string? AvatarUrl { get; set; }
    /// <summary>
    /// User's roles
    /// </summary>
    public IList<string> Roles { get; set; }
}