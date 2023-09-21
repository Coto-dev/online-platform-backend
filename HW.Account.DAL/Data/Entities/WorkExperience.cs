using System.ComponentModel.DataAnnotations.Schema;
using HW.Common.Enums;

namespace HW.Account.DAL.Data.Entities; 

public class WorkExperience {
    public Guid Id { get; set; } = Guid.NewGuid();
    public required User User { get; set; }
    public List<WorkExperienceInfo>? WorkExperiencesInfos { get; set; } = new();
    public ProfileVisibility Visibility { get; set; } = ProfileVisibility.All;
}