using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ehs5.Auth { 
    public class GenerateToken
    {
        public static string TokenGenerator(string key, List<KeyValuePair<string, string>> claims, int time)
        {
            var secret = Encoding.UTF8.GetBytes(key);

            var claimslist = new List<Claim>();

            foreach (var claim in claims)
            {
                claimslist.Add(new Claim(ClaimTypes.Role, claim.Value));
            }

            var hanlder = new JwtSecurityTokenHandler();

            var Token = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:7098/",
                Audience = "https://localhost:7098/",
                IssuedAt = DateTime.UtcNow,
                Expires = time == 0 ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(time),
                Subject = new ClaimsIdentity(claimslist),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), "HS256")
            };



            return hanlder.CreateEncodedJwt(Token);
        }
    }
}
