using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace  SapirProductionFloorManagment.Shared
{
    public record Line
    {
        public int LineNumber { get; set; }
        public string? Name { get; set; }
        public WorkOrder WorkOrder { get; set; } = new();
        public int ProductionRate { get; set; }
        public string ShiftStartWork { get; set; } = "Unspecified";
        public string ShiftEndWork { get; set; } = "Unspecified";
        public bool[] WorkDays { get; set; } = new bool[] { true, true, true, true, true, false, false };

    }
}
