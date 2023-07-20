using System.ComponentModel.DataAnnotations.Schema;
using HW.Common.Enums;

namespace HW.Account.DAL.Data.Entities; 

public class BirthDate {
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("UserId")]
    public User User;
    public DateTime? Value { get; set; }
    public ProfileVisibility Visibility { get; set; } = ProfileVisibility.All;
}