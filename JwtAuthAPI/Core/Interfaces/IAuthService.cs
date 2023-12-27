using JwtAuthAPI.Core.Dtos;

namespace JwtAuthAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponseDto> SeedRoleAsync();
        Task<AuthServiceResponseDto> RegisterAsync(RegisterDto register);
        Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto UpdatePermissionDto);
        Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto);
    }
}
