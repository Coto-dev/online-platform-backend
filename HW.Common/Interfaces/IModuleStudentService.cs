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
    public Task<string> CalculateProgress(Guid moduleId, Guid userId);
    public Task<ChapterFullDto> GetChapterContent(Guid chapterId, Guid userId);
    public Task<ModuleDetailsDto> GetModuleDetails(Guid moduleId, Guid? userId);
    public Task<ModuleDetailsDto> SendCommentToModule(ModuleCommentDto model, Guid moduleId, Guid userId);
    public Task BuyModule(Guid moduleId, Guid userId);
    public Task StartModule(Guid moduleId, Guid userId);
    public Task AddModuleToBasket(Guid moduleId, Guid userId);
    public Task DeleteModuleFromBasket(Guid moduleId, Guid userId);

}