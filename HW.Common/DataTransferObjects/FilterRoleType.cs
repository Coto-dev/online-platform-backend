using HW.Common.Enums;

namespace HW.Common.DataTransferObjects;

public class FilterRoleType
{
    public List<RoleType>? RoleTypes { get; set; } = new List<RoleType>((RoleType[])Enum.GetValues(typeof(RoleType)));
}