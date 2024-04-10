using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  SapirProductionFloorManagment.Shared
{
    public record WorkOrder
    {
        public Customer? Customer { get; set; }
        public int Id { get; set; }
        public int ProductNumber { get; set; }
        public string ProductName { get; set; } = ""; 
        public DateTime CreateDate { get; set; }
        public string Description { get; set; } = "";
        public int Weigth { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double WorkDuration { get; set; }
        public string Comments { get; set; } = "";

        //public DateTime? DueDate { get; set;} 

    }
}
