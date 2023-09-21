using System.ComponentModel.DataAnnotations.Schema;
using HW.Common.Enums;

namespace HW.Account.DAL.Data.Entities; 

public class Location {
    public Guid Id { get; set; } = Guid.NewGuid();
    public required User User;
    public string? Place { get; set; }
    public ProfileVisibility Visibility { get; set; } = ProfileVisibility.All;
}