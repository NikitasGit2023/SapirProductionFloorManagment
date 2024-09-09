using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirProductionFloorManagment.Shared
{
    public record WorkOrdersTableContext
    {

            public int Id { get; set; }
            public string WorkOrderSN { get; set; } = "";
            public string ProductDesc { get; set; } = "";
            public int Quantity { get; set; }
            public int OptionalLine1 { get; set; }
            public int OptionalLine2 { get; set; }
            public int Priority { get; set; }
            public string Comments { get; set; } = "";
            public DateTime CompletionDate { get; set; }
            public int SizeInMicron { get; set; }
    }
}
