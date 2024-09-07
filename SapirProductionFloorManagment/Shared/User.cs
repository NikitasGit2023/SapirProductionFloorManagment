using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace  SapirProductionFloorManagment.Shared
{
    public record User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;    
        public string? Password { get; set; }
        public string Role { get; set; } = string.Empty;    
        public string JobTitle { get; set; } = string.Empty;    

       
    

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

    }
}
