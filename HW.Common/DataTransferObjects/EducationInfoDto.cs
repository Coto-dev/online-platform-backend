namespace HW.Common.DataTransferObjects; 

public class EducationInfoDto {
    public Guid Id { get; set; }
    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialization { get; set; }
    public string? Status { get; set; }
}