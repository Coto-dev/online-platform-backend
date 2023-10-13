namespace HW.Common.DataTransferObjects; 

public class GradeGraph {
    public UserProgress UserProgress { get; set; }
    public int WorksCount { get; set; }
    public List<GradeGraphSubModule> SubModules { get; set; } = new();
}