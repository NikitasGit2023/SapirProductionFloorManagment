using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirProductionFloorManagment.Shared
{
    public class WorkOrder
    {

            public int Id { get; set; }
            public string WorkOrderSN { get; set; } = "";
            public string ProductDesc { get; set; } = "";
            public int QuantityInKg { get; set; }
            public string OptionalLine1 { get; set; } = "" ;
            public string OptionalLine2 { get; set; } = ""; 
            public int Priority { get; set; }
            public string Comments { get; set; } = "";
            public DateTime DeadLineDateTime { get; set; } = DateTime.Now;
            public int SizeInMicron { get; set; }
            public int ProducedQuantity { get; set; }
            public int QuantityLeft { get; set;}
            public string? Status { get; set; } = null;



            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }


    }
}
