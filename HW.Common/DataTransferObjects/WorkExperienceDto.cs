using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class WorkExperienceDto {
    public List<WorkExperienceInfoDto> WorkExperienceInfoDtos { get; set; }
    public ProfileVisibility Visibility { get; set; }
}