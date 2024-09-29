using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirProductionFloorManagment.Shared
{
    public record LineWorkPlan
    {
        public int Id { get; set; }
        public string RelatedToLine { get; set; } = "";
        public DateTime? StartWork { get; set; } = null;
        public DateTime? EndWork { get; set; } = null;
        public int Priority { get; set;}
        public  DateTime? DeadLineDateTime {  get; set; } = null;       
        public double WorkDuraion { get; set; }
        public string FormatedWorkDuration { get; set; } = "";            
        public double LeftToFinish { get; set; }
        public string FormatedLeftToFinish { get; set; } = "";   
        public int QuantityInKg { get; set; }
        public string Description { get; set; } = "";
        public string WorkOrderSN { get; set; } = "";
        public string Comments { get; set; } = "";
        public int SizeInMicron {get; set;} 
        public string? Status { get; set; }



        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


    }
}
