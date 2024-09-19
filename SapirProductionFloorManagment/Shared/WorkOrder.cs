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
            public int Quantity { get; set; }
            public string OptionalLine1 { get; set; } = "" ;
            public string OptionalLine2 { get; set; } = ""; 
            public int Priority { get; set; }
            public string Comments { get; set; } = "";
            public DateTime CompletionDate { get; set; } = DateTime.Now;
            public int SizeInMicron { get; set; }
            public int ProducedQuantity { get; set; }
            public int QuantityLeft { get; set;}


            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }


    }
}
