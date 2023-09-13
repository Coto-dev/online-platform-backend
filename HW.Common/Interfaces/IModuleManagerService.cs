using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;

namespace HW.Common.Interfaces; 

public interface IModuleManagerService {
    Task<PagedList<ModuleShortDto>> GetTeacherModules(PaginationParamsDto pagination, FilterModuleType? filter,
        ModuleFilterTeacherType? section, string? sortByNameFilter, SortModuleType? sortModuleType, Guid userId);
    public Task<ModuleFullTeacherDto> GetModuleContent(Guid moduleId, Guid userId);
    public Task<ChapterFullTeacherDto> GetChapterContent(Guid chapterId, Guid userId);
    Task EditSubModulesOrder(List<Guid> orderedSubModules, Guid moduleId);
    Task EditChaptersOrder(List<Guid> orderedChapters, Guid subModuleId);

    Task CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId);
    Task EditSelfStudyModule(ModuleSelfStudyEditDto model, Guid moduleId, Guid userId);
    Task EditVisibilityModule(ModuleVisibilityType visibilityType, Guid moduleId);
    Task CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId);
    Task EditStreamingModule(ModuleStreamingEditDto model, Guid moduleId, Guid userId);
    Task ArchiveModule(Guid moduleId);
    Task AddSubModule(Guid moduleId, SubModuleCreateDto model);
    Task EditSubModule(Guid subModuleId, SubModuleEditDto model);
    Task ArchiveSubModule(Guid subModuleId);
    Task CreateChapter(Guid subModuleId, ChapterCreateDto model);
    Task EditChapter(Guid chapterId, ChapterEditDto model);
    Task ArchiveChapter(Guid chapterId);
    
}