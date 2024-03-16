namespace HW.Common.DataTransferObjects;

public class ModuleCommentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsTeacherComment { get; set; }
    public string Message { get; set; }
}
