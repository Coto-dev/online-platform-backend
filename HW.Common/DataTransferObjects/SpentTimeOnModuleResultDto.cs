using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Common.DataTransferObjects;

public class SpentTimeOnModuleResultDto
{
    public Guid StudentId { get; set; }
    public Guid ModuleId { get; set; }
    public SpentTimeDto SpentTimeDto { get; set; }
}
