namespace HW.Common.DataTransferObjects; 

public class RequiredModulesDto {
    public Guid Id { get; set; }
    public FileLinkDto? Avatar { get; set; }
    public string Name { get; set; }
}