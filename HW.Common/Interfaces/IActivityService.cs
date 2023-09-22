using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces;

public interface IActivityService
{
    Task<YearActivityDto> GetUserActivity(Guid userId);
}