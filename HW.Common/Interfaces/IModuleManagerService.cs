using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;

namespace HW.Common.Interfaces; 

public interface IModuleManagerService {
    Task<PagedList<ModuleShortDto>> GetTeacherModules(PaginationParamsDto pagination, FilterModuleType? filter,
        ModuleTeacherFilter? section, string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId);
    public Task<ModuleFullTeacherDto> GetModuleContent(Guid moduleId, Guid userId);
    Task EditChapterTestsOrder(List<Guid> orderedChapterTests, Guid chapterId);
    Task CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId);
    Task EditSelfStudyModule(ModuleSelfStudyEditDto model, Guid moduleId, Guid userId);
    Task EditVisibilityModule(ModuleVisibilityType visibilityType, Guid moduleId);
    Task CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId);
    Task EditStreamingModule(ModuleStreamingEditDto model, Guid moduleId, Guid userId);
    Task ArchiveModule(Guid moduleId);
    
    
}