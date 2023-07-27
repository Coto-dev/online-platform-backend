using HW.Backend.BL.Extensions;
using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services; 

public class ModuleStudentService : IModuleStudentService {
    private readonly ILogger<ModuleStudentService> _logger;
    private readonly BackendDbContext _dbContext;

    public ModuleStudentService(BackendDbContext dbContext, ILogger<ModuleStudentService> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedList<ModuleShortDto>> GetAvailableModules(PaginationParamsDto pagination, FilterModuleType? filter, string? sortByNameFilter,
        SortModuleType? sortModuleType, Guid userId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");
        
        var modules = _dbContext.Modules
            .Where(m => !m.ArchivedAt.HasValue)
            .ModuleAvailableFilter(filter,sortByNameFilter)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        
        var shortModules = modules.Select(x =>new ModuleShortDto {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
            ModuleStatusType =  
                user == null ? null : 
                user.Modules!.All(m => m.Module != x) ? null :
                user.Modules!.FirstOrDefault(m=>m.Module == x)!.ModuleStatus == ModuleStatusType.InCart ? ModuleStatusType.InCart :
                ModuleStatusType.Purchased
        });
        return await PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<PagedList<ModuleShortDto>> GetStudentModules(PaginationParamsDto pagination, FilterModuleType? filter, string? sortByNameFilter,
        ModuleFilterStudentType? section, SortModuleType? sortModuleType, Guid userId) {
        if (pagination.PageNumber <= 0)
            throw new BadRequestException("Wrong page");
        
        var modules = _dbContext.Modules
            .Where(m => !m.ArchivedAt.HasValue)
            .ModuleStudentFilter(filter, section, sortByNameFilter, userId)
            .ModuleOrderBy(sortModuleType)
            .AsQueryable()
            .AsNoTracking();
        
        var shortModules = modules.Select(x =>new ModuleShortDto {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            Status = typeof(Module) == x.GetType()? ModuleType.SelfStudyModule : ModuleType.StreamingModule,
        });
        return await PagedList<ModuleShortDto>.ToPagedList(shortModules, pagination.PageNumber, pagination.PageSize);    }

    public async Task<ModuleFullDto> GetModuleContent(Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task<ChapterFullDto> GetChapterContent(Guid chapterId) {
        throw new NotImplementedException();
    }

    public async Task<ModuleDetailsDto> GetModuleDetails(Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task<ModuleDetailsDto> SendCommentToModule(ModuleCommentDto model, Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task<ModuleDetailsDto> BuyModule(Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task AddModuleToBasket(Guid moduleId) {
        throw new NotImplementedException();
    }

    public async Task DeleteModuleToBasket(Guid moduleId) {
        throw new NotImplementedException();
    }
}