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
        public string? FullName { get; set; }            
        public string? JobTitle { get; set; }   
        public string? Permission { get; set;}     
        public string?  Password { get; set; }

    }
}
