using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;
using Microsoft.AspNetCore.Mvc;

namespace HW.Common.Interfaces; 

public interface IModuleStudentService {
    public Task<PagedList<ModuleShortDto>>GetAvailableModules(PaginationParamsDto pagination, FilterModuleType? filter,
        string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId);
    public Task<PagedList<ModuleShortDto>> GetStudentModules(PaginationParamsDto pagination, FilterModuleType? filter,
         string? sortByNameFilter, ModuleFilterStudentType? section, SortModuleType? sortModuleType, Guid userId);
    public Task<ModuleFullDto> GetModuleContent(Guid moduleId);
    public Task<ChapterFullDto> GetChapterContent(Guid chapterId);
    public Task<ModuleDetailsDto> GetModuleDetails(Guid moduleId);
    public Task<ModuleDetailsDto> SendCommentToModule(ModuleCommentDto model, Guid moduleId);
    public Task<ModuleDetailsDto> BuyModule(Guid moduleId);
    public Task AddModuleToBasket(Guid moduleId);
    public Task DeleteModuleToBasket(Guid moduleId);

}