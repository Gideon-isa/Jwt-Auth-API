using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuthAPI.Core.JwtProvider
{
    public interface IJwtProvider
    {
        string GenerateToken(IdentityUser user, List<string> userRoles);
    }
}
