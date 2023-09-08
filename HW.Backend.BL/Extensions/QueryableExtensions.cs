using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;

namespace HW.Backend.BL.Extensions; 

public static  class QueryableExtensions {
    public static IQueryable<Module> ModuleOrderBy(this IQueryable<Module> source, SortModuleType? sort) {
        return sort switch {
            SortModuleType.PriceAsc=> source.OrderBy(x => x.Price),
            SortModuleType.PriceDesc => source.OrderByDescending(x => x.Price),
            SortModuleType.NameAsc=> source.OrderBy(x => x.Name),
            SortModuleType.NameDesc => source.OrderByDescending(x => x.Name),
            SortModuleType.CreationTimeAsc=> source.OrderBy(x => x.CreatedAt),
            SortModuleType.CreationTimeDesc => source.OrderByDescending(x => x.CreatedAt),
            _ => source
        };
    }
    
    /*public static IQueryable<StreamingModule> StreamingModuleOrderBy(this IQueryable<StreamingModule> source, SortModuleType? sort) {
        return sort switch {
            SortModuleType.StartTimeAsc=> source.OrderBy(x => x.StartAt),
            SortModuleType.StartTimeDesc => source.OrderByDescending(x => x.StartAt),
            SortModuleType.ExpirationTimeAsc=> source.OrderBy(x => x.ExpiredAt),
            SortModuleType.ExpirationTimeDesc => source.OrderByDescending(x => x.ExpiredAt),
            _ => source
        };
    }*/
    
    public static IQueryable<Module> ModuleTeacherFilter(this IQueryable<Module> source, FilterModuleType? filter,
        ModuleFilterTeacherType? section, string? sortByNameFilter, Guid userId) {

        return source.Where(x =>
            (filter == null || (filter.ModuleTypes!.Contains(ModuleType.StreamingModule) 
                               && x.GetType() == typeof(StreamingModule)) 
                           || (filter.ModuleTypes!.Contains(ModuleType.SelfStudyModule) 
                               && x.GetType() == typeof(Module)))
                           && (section == null || (section == ModuleFilterTeacherType.Draft
                                                   && x.ModuleVisibility == ModuleVisibilityType.OnlyCreators
                                                   && x.Editors!.Any(c=>c.Id == userId))
                           || (section == ModuleFilterTeacherType.Published
                               && x.Editors!.Any(c=>c.Id == userId)
                               && x.ModuleVisibility == ModuleVisibilityType.Everyone)
                           || (section == ModuleFilterTeacherType.Taught && x.Teachers!.Any(t=>t.Id == userId)))
                           && (sortByNameFilter == null || x.Name.ToLower().Contains(sortByNameFilter.ToLower())));
    }
    
    public static IQueryable<Module> ModuleStudentFilter(this IQueryable<Module> source, FilterModuleType? filter,
        ModuleStudentFilter? section, string? sortByNameFilter, Guid userId) {
        return source.Where(x =>
            (filter == null || (filter.ModuleTypes!.Contains(ModuleType.StreamingModule) 
                                && x.GetType() == typeof(StreamingModule)) 
                            || (filter.ModuleTypes!.Contains(ModuleType.SelfStudyModule) 
                                && x.GetType() == typeof(Module)))
            && (section == null || (section.ModuleStudentTypes!.Contains(ModuleFilterStudentType.Passed)
                                    && x.UserModules!.Any(u=>u.Student.Id == userId && u.ModuleStatus == ModuleStatusType.Passed))
                                || (section.ModuleStudentTypes!.Contains(ModuleFilterStudentType.NotPassed) 
                                    && x.UserModules!.Any(u=>u.Student.Id == userId 
                                                             && (u.ModuleStatus == ModuleStatusType.NotPassedByExam 
                                                                 || u.ModuleStatus == ModuleStatusType.NotPassedByExpiration)))
                                || (section.ModuleStudentTypes!.Contains(ModuleFilterStudentType.InProcess)
                                                                  && x.UserModules!.Any(u=>u.Student.Id == userId && u.ModuleStatus == ModuleStatusType.InProcess))
                                || (section.ModuleStudentTypes!.Contains(ModuleFilterStudentType.InCart)
                                    && x.UserModules!.Any(u=>u.Student.Id == userId && u.ModuleStatus == ModuleStatusType.InCart))
                                || (section.ModuleStudentTypes!.Contains(ModuleFilterStudentType.Purchased)
                                    && x.UserModules!.Any(u=>u.Student.Id == userId && u.ModuleStatus == ModuleStatusType.Purchased)))
            && (sortByNameFilter == null || x.Name.ToLower().Contains(sortByNameFilter.ToLower())));        
    }
    
    public static IQueryable<Module> ModuleAvailableFilter(this IQueryable<Module> source, FilterModuleType? filter,
        string? sortByNameFilter) {
        return source.Where(x =>
            (filter == null || (filter.ModuleTypes!.Contains(ModuleType.StreamingModule) 
                                && x.GetType() == typeof(StreamingModule)) 
                            || (filter.ModuleTypes!.Contains(ModuleType.SelfStudyModule) 
                                && x.GetType() == typeof(Module)))
            && (sortByNameFilter == null || x.Name.ToLower().Contains(sortByNameFilter.ToLower())));
            
    }
}