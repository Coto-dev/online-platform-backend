using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class GradeGraphSubModule {
    public Guid SubModuleId { get; set; }
    public string SubModuleName { get; set; }
    public List<GradeGraphChapter> Chapters { get; set; } = new();
    public UserAnswerTestStatus GraphElementStatus { get; set; }
}