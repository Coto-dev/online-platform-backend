using System.ComponentModel.DataAnnotations;

namespace HW.Common.DataTransferObjects; 

public class ChapterBlockEditDto {
    public string? Content { get; set; }
    public List<string>? FileIds { get; set; } = new();
}