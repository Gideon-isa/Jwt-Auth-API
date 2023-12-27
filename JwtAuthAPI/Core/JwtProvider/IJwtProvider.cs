using JwtAuthAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuthAPI.Core.JwtProvider
{
    public interface IJwtProvider
    {
        string GenerateToken(ApplicationUser user, List<string> userRoles);
    }
}
