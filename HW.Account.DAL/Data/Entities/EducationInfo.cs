namespace HW.Account.DAL.Data.Entities; 

public class EducationInfo {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Education Education { get; set; }
    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialization { get; set; }
    public string? Status { get; set; }
    public DateTime? FinishAt { get; set; }
}