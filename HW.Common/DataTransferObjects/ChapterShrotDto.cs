using HW.Common.Enums;

namespace HW.Common.DataTransferObjects; 

public class ChapterShrotDto {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ChapterType ChapterType { get; set; }
    public bool IsLearned { get; set; }
}