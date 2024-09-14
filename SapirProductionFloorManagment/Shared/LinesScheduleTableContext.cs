using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirProductionFloorManagment.Shared
{
    public record LinesScheduleTableContext
    {
        public int Id { get; set; }     
        public string? RelatedToLine { get; set; }       
        public DateTime StartWork { get; set; } 
        public DateTime EndWork { get; set; }   
        public double WorkDuraion { get; set; }
        public int QuantityInKg { get; set; }   
        public string? Description { get; set; }
        public string WorkOrderSN { get; set; } = "";
        public string? Comments { get; set; } = "";
        public  DateTime TimeToFinish { get; set; } 
        public int SizeInMicron {get; set;}       
        public bool IsCalculted { get; set; }


    }
}
