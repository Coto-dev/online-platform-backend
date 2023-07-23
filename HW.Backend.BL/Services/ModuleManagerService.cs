using HW.Backend.DAL.Data;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services; 

public class ModuleManagerService : IModuleManagerService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;

    public ModuleManagerService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task<PagedList<ModuleShortDto>> GetTeacherModules(PaginationParamsDto pagination, ModuleFilterTeacherType? section, Guid userId) {
        throw new NotImplementedException();
    }

    public async Task CreateSelfStudyModule(ModuleSelfStudyCreateDto model, Guid userId) {
        throw new NotImplementedException();
    }

    public async Task EditSelfStudyModule(ModuleSelfStudyEditDto model) {
        throw new NotImplementedException();
    }

    public async Task CreateStreamingModule(ModuleStreamingCreateDto model, Guid userId) {
        throw new NotImplementedException();
    }

    public async Task EditStreamingModule(ModuleStreamingEditDto model) {
        throw new NotImplementedException();
    }

    public async Task ArchiveModule(Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task AddSubModule(Guid moduleId, SubModuleCreateDto model) {
        throw new NotImplementedException();
    }

    public async Task EditSubModule(Guid subModuleId, SubModuleEditDto model) {
        throw new NotImplementedException();
    }

    public async Task ArchiveSubModule(Guid subModuleId) {
        throw new NotImplementedException();
    }

    public async Task CreateChapter(Guid subModuleId, ChapterCreateDto model) {
        throw new NotImplementedException();
    }

    public async Task EditChapter(Guid chapterId, ChapterEditDto model) {
        throw new NotImplementedException();
    }

    public async Task ArchiveChapter(Guid chapterId) {
        throw new NotImplementedException();
    }
}