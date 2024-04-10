using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  SapirProductionFloorManagment.Shared
{
    public record Customer
    {
        public int? CustomerId { get; set; }
        public string? Name { get; set; }
        public int? SalesOrderNumber { get; set; }

    }
}
