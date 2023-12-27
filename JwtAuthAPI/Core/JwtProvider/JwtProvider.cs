using JwtAuthAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthAPI.Core.JwtProvider
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtProvider(IOptions<JwtOptions> option)
        {
            _jwtOptions = option.Value;
        }


        public string GenerateToken(ApplicationUser user, List<string> userRoles)
        {

            var authClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // adding the user's role if any
            foreach (var userRole in userRoles)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // generate the encrypted secret key
            SigningCredentials secretKey
                =  new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey!)),
                SecurityAlgorithms.HmacSha256);

            // generate a token
            JwtSecurityToken token 
                =  new JwtSecurityToken
                (_jwtOptions.Issuer, _jwtOptions.Audience, authClaim, null, DateTime.UtcNow.AddHours(1), 
                secretKey);

          

            return new JwtSecurityTokenHandler().WriteToken(token);

             
        }

        
    }
}
