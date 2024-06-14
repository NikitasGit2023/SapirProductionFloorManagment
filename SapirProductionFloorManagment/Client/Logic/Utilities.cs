using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Client.Logic
{
    public record Utilities
    {
        public void ConvertToWorkDays(Line line)
        {
            for (int i = 0; i<line.WorkDays.Count(); i++)
            {
                if (line.WorkDays[i] is true)
                {
                    line.NumericWorkDay++;
                }
            }


        }
    }
}
