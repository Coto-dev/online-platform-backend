using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;
using Microsoft.AspNetCore.Mvc;

namespace HW.Common.Interfaces; 

public interface IModuleStudentService {
    public Task<PagedList<ModuleShortDto>>GetAvailableModules(PaginationParamsDto pagination, FilterModuleType? filter,
        string? sortByNameFilter, SortModuleType? sortModuleType, Guid? userId);
    public Task<PagedList<ModuleShortDto>> GetStudentModules(PaginationParamsDto pagination, FilterModuleType? filter,
         string? sortByNameFilter, ModuleStudentFilter? section, SortModuleType? sortModuleType, Guid userId);
    public Task<ModuleFullDto> GetModuleContent(Guid moduleId, Guid userId);
    public Task<float> CalculateProgressFloat(Guid moduleId, Guid userId);
    public Task<ModuleDetailsDto> GetModuleDetails(Guid moduleId, Guid? userId);
    public Task BuyModule(Guid moduleId, Guid userId);
    public Task StartModule(Guid moduleId, Guid userId);
    public Task AddModuleToBasket(Guid moduleId, Guid userId);
    public Task DeleteModuleFromBasket(Guid moduleId, Guid userId);
    public Task AddSpentTimeOnModule(Guid moduleId, Guid userId, SpentTimeDto spentTime);
    public Task SendCommentToModule(ModuleCommentCreateDto model, Guid moduleId, Guid userId);
    public Task DeleteCommentFromModule(Guid commentId, Guid userId);
    public Task EditCommentInModule(ModuleCommentEditDto message, Guid commentId, Guid userId);
    public Task<PagedList<ModuleCommentDto>> GetModuleComments(Guid moduleId, PaginationParamsDto pagination);

}