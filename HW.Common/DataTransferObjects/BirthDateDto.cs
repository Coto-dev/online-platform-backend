using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class BirthDateDto {
    [Range(typeof(DateTime), "01/01/1900", "01/01/2023")]
    public DateTime? Value { get; set; }
    [DefaultValue(ProfileVisibility.All)]
    public ProfileVisibility Visibility { get; set; }
}