using Microsoft.IdentityModel.Tokens;
using SapirProductionFloorManagment.Shared;
using SapirProductionFloorManagment.Shared.Authentication___Autherization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SapirProductionFloorManagment.Server.Authentication
{   
    public class JwtAuthenticationManager
    {
        
        public ILogger Logger { get; set;} 

        public const string JWT_SECURITY_KEY = "NikitaDanielLiliaYuriMurkinRijick1234567890";

        public const int JWT_VALIDITY_TIME_IN_MINS = 580;

        private User _user { get; set; }        

        public JwtAuthenticationManager(User user)
        {
            _user = user;  
        }

        public UserSession? GenerateJwtToken(string name, string password)
        {

            var userSession = new UserSession();    
            try
            {

                if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(password))
                    return null;

                var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_VALIDITY_TIME_IN_MINS);
                var tokeKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
                var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.FullName),
                new Claim(ClaimTypes.Role,_user.Role)

            });

                var signInCredentials = new SigningCredentials
                    (
                        new SymmetricSecurityKey(tokeKey),
                        SecurityAlgorithms.HmacSha256

                    );

                var securityKeyDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = claimsIdentity,
                    Expires = tokenExpiryTimeStamp,
                    SigningCredentials = signInCredentials

                };


                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var securityToken = jwtSecurityTokenHandler.CreateToken(securityKeyDescriptor);
                var token = jwtSecurityTokenHandler.WriteToken(securityToken);

                userSession = new UserSession
                {
                    UserName = _user.FullName,
                    Role = _user.Role,
                    Token = token,
                    ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
                };


            }
            catch(Exception ex) 
            {
                Logger.LogError(ex.Message);
            }
            return userSession;

        }

    }
}
    