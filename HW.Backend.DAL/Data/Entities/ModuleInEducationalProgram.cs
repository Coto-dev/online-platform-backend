using HW.Backend.DAL.Data.Entities;

namespace HW.Backend.Dal.Data.Entities;

/// <summary>
/// Entity for ModuleInEducationalProgram
/// </summary>
public class ModuleInEducationalProgram
{
	/// <summary>
	/// ModuleInEducationalProgram identifier
	/// </summary>
	public Guid Id { get; set; }
	/// <summary>
	/// List of modules
	/// </summary>
	public required Module Module { get; set; }
	/// <summary>
	/// EducationalProgram identifier
	/// </summary>
	public required EducationalProgram EducationalProgram { get; set; }
	/// <summary>
	/// List of required modules
	/// </summary>
	public List<Module>? RequRequiredModules { get; set; }

}
