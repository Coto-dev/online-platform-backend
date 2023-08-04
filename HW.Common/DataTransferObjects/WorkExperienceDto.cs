using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class WorkExperienceDto {
    public List<WorkExperienceInfoDto> WorkExperienceInfos { get; set; }
    public ProfileVisibility Visibility { get; set; }
}