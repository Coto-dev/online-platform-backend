using HW.Account.DAL.Data.Entities;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;

namespace HW.AdminPanel.BL.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<Module> ModuleOrderBy(this IQueryable<Module> source, SortModuleType? sort)
    {
        return sort switch {
            SortModuleType.PriceAsc => source.OrderBy(x => x.Price),
            SortModuleType.PriceDesc => source.OrderByDescending(x => x.Price),
            SortModuleType.NameAsc => source.OrderBy(x => x.Name),
            SortModuleType.NameDesc => source.OrderByDescending(x => x.Name),
            SortModuleType.CreationTimeAsc => source.OrderBy(x => x.CreatedAt),
            SortModuleType.CreationTimeDesc => source.OrderByDescending(x => x.CreatedAt),
            _ => source
        };
    }

    public static IQueryable<Module> ModuleAdminFilter(this IQueryable<Module> source, FilterModuleType? filter,
        string? sortByNameFilter) {

        return source.Where(x =>
            (filter == null || (filter.ModuleTypes!.Contains(ModuleType.StreamingModule)
                               && x.GetType() == typeof(StreamingModule))
                           || (filter.ModuleTypes!.Contains(ModuleType.SelfStudyModule)
                               && x.GetType() == typeof(Module)))
                           && (sortByNameFilter == null || x.Name.ToLower().Contains(sortByNameFilter.ToLower())));
    }

    public static IQueryable<User> UserRoleFilter(this IQueryable<User> source,  FilterRoleType? roleFilter) {
        return source.Where(u =>
            (roleFilter!.RoleTypes!.Contains(RoleType.Student) && u.Roles.All(r => r.Role.RoleType == RoleType.Student))
            || (roleFilter!.RoleTypes!.Contains(RoleType.Teacher) && u.Roles.Any(r =>
                                                                      r.Role.RoleType == RoleType.Teacher)
                                                                  && u.Roles.All(r =>
                                                                      r.Role.RoleType != RoleType.Administrator))
            || (roleFilter!.RoleTypes!.Contains(RoleType.Administrator) &&
                u.Roles.Any(r => r.Role.RoleType == RoleType.Administrator)));
    }
}