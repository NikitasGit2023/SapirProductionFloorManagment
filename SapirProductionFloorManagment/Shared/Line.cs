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
        public string Name { get; set; } = "";
        public int ProductionRate { get; set; }


        public override int GetHashCode()
        {
            return LineId.GetHashCode();
        }


    }
}
