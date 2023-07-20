using System.ComponentModel;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class LocationDto {
    public string?  Place { get; set; }
    [DefaultValue(ProfileVisibility.All)]
    public ProfileVisibility Visibility { get; set; }
}