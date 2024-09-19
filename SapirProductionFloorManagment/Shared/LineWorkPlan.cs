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
        public DateTime StartWork { get; set; } = DateTime.Now; 
        public DateTime EndWork { get; set; } = DateTime.Now;   
        public double WorkDuraion { get; set; }
        public string FormatedWorkDuration { get; set; } = "";      
        public int QuantityInKg { get; set; }
        public string Description { get; set; } = "";
        public string WorkOrderSN { get; set; } = "";
        public string Comments { get; set; } = "";
        public  DateTime TimeToFinish { get; set; } = DateTime.Now;
        public int SizeInMicron {get; set;}       
        public bool IsCalculted { get; set;}


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


    }
}
