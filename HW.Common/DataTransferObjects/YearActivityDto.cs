namespace HW.Common.DataTransferObjects;

public class YearActivityDto
{
    public required List<DayActivityDto> DayActivities { get; set; }
    public int MaxActivity { get; set; }

}