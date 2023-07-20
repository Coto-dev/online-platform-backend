using System.ComponentModel.DataAnnotations.Schema;
using HW.Common.Enums;

namespace HW.Account.DAL.Data.Entities; 

public class PhotoId {
    public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey("UserId")]
    public User User;
    public string? PhotoName { get; set; }
    public ProfileVisibility Visibility { get; set; } = ProfileVisibility.All;
}