using System.ComponentModel;

namespace HW.Common.DataTransferObjects; 

public class PaginationParamsDto {
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;
    [DefaultValue(6)]
    public int PageSize { get; set; } = 6;
}