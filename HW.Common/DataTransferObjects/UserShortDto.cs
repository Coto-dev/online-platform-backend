using HW.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HW.Common.DataTransferObjects;

public class UserShortDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string NickName { get; set; }
    public string? AvatarId { get; set; }
    public string Email { get; set; }
    public string? Role { get; set; }
    public bool IsEmailConfirm { get; set; }
    public bool IsBanned { get; set; }
    public List<ModuleUserRoleType> UserModuleRoles { get; set; } = new();
}