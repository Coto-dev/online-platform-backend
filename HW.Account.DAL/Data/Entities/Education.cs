using System.ComponentModel.DataAnnotations.Schema;
using HW.Common.Enums;

namespace HW.Account.DAL.Data.Entities; 

public class Education {
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public required User User { get; set; }
    public List<EducationInfo>? EducationInfos { get; set; } = new();
    public ProfileVisibility Visibility { get; set; } = ProfileVisibility.All;
}