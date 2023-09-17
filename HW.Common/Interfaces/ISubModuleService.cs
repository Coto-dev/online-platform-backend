using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces; 

public interface ISubModuleService {
    Task AddSubModule(Guid moduleId, SubModuleCreateDto model);
    Task EditSubModule(Guid subModuleId, SubModuleEditDto model);
    Task ArchiveSubModule(Guid subModuleId);
    Task EditSubModulesOrder(List<Guid> orderedSubModules, Guid moduleId);

}