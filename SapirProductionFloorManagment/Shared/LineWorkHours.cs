using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirProductionFloorManagment.Shared
{
    public record LineWorkHours
    {
        public int Id { get; set; }
        public string ReferencedToLine { get; set; } = "";
        public string WorkDay { get; set; } = "Unspecified";
        public string ShiftStartWork { get; set; } = "Unspecified";
        public string ShiftEndWork { get; set; } = "Unspecified";
        public string BreakStart { get; set; } = "Unspecified";
        public string BreakEnd { get; set; } = "Unspecified";

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }
}
