using HW.Common.Enums;

namespace HW.Common.DataTransferObjects;

public class ModuleTagsDto
{
    public List<Guid>? TagsId { get; set; } = new List<Guid>();
}
