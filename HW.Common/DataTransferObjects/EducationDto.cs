using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class EducationDto {
    public List<EducationInfoDto>? EducationInfos { get; set; } = new();
    public ProfileVisibility Visibility { get; set; }
}