using System.ComponentModel;
using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class PhotoIdDto {
    public string? PhotoName { get; set; }
    [DefaultValue(ProfileVisibility.All)]
    public ProfileVisibility Visibility { get; set; }
}