using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;

namespace HW.Common.Interfaces; 

public interface IModuleManagerService {
    Task<PagedList<ModuleShortDto>> GetTeacherModules(PaginationParamsDto pagination, FilterModuleType? filter,
        ModuleFilterTeacherType? section, string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId);
    Task CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId);
    Task EditSelfStudyModule(ModuleSelfStudyEditDto model);
    Task CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId);
    Task EditStreamingModule(ModuleStreamingEditDto model);
    Task ArchiveModule(Guid moduleId);
    Task AddSubModule(Guid moduleId, SubModuleCreateDto model);
    Task EditSubModule(Guid subModuleId, SubModuleEditDto model);
    Task ArchiveSubModule(Guid subModuleId);
    Task CreateChapter(Guid subModuleId, ChapterCreateDto model);
    Task EditChapter(Guid chapterId, ChapterEditDto model);
    Task ArchiveChapter(Guid chapterId);
    
}