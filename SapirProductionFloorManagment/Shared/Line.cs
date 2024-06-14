using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  SapirProductionFloorManagment.Shared
{
    public record Line
    {
        public int LineId { get; set; } 
        public string? Name { get; set; }
        public WorkOrder WorkOrder { get; set; } = new();
        public string ShiftStartWork { get; set; } = "Unspecified";
        public string ShiftEndWork { get; set; } = "Unspecified";
        public int ProductionRate { get; set; }
        public int NumericWorkDay { get; set; }
        public DateTime WorkDate { get; set; }
       
        [NotMapped]
        public bool[] WorkDays { get; set; } = new bool[] 
        {
            true, true, true, true, true, false, false 
        };
           

    }
}
