using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class GradeGraphChapter {
    public Guid ChapterId { get; set; }
    public string ChapterName { get; set; }
    public UserAnswerTestStatus GraphElementStatus { get; set; }
}