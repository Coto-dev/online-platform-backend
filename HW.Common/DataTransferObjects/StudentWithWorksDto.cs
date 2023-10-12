namespace HW.Common.DataTransferObjects; 

public class StudentWithWorksDto {
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public int WorksCount { get; set; }
}