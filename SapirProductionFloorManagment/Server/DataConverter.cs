namespace SapirProductionFloorManagment.Server
{
    public class DataConverter
    {


        public string ConvertToTimeString(double hours)
        {
            // Split hours into hours and minutes
            int hrs = (int)Math.Floor(hours);
            int mins = (int)((hours - hrs) * 60);

            // Format the time as HH:mm
            return $"{hrs:D2}:{mins:D2}";
        }
    }
}
