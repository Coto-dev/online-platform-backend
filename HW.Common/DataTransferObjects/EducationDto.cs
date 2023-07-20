using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class EducationDto {
    public List<EducationInfoDto>? EducationInfoDtos { get; set; }
    public ProfileVisibility Visibility { get; set; }
}