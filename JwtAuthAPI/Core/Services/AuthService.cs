using JwtAuthAPI.Core.Dtos;
using JwtAuthAPI.Core.Entities;
using JwtAuthAPI.Core.Interfaces;
using JwtAuthAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JwtAuthAPI.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IJwtProvider jwtProvider, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }


        public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            //gets the user by username
            ApplicationUser? user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid credentials"
                };
            }

            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid credentials"
                };
            }

            var userRoles = (List<string>)await _userManager.GetRolesAsync(user);

            string token = _jwtProvider.GenerateToken(user, userRoles);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = token
            };
        }

        public async Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto UpdatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(UpdatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid credentials"
                };
            }

            var taskResult = await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "User is now and Admin"
            };
        }

        public async Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid User"
                };
            }

            var taskResult = await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "User is now and Owner"
            };
        }

        public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto register)
        {
            var isExistsUser = await _userManager.FindByNameAsync(register.UserName);

            if (isExistsUser != null)
            {
                return new AuthServiceResponseDto() { IsSucceed = false, Message = "Username already exists" };
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                UserName = register.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser, register.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation Failed Because: ";

                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new AuthServiceResponseDto() { IsSucceed = false, Message = errorString };
            }

            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            return new AuthServiceResponseDto() { IsSucceed = false, Message = "User Created Successfully" };
        }

        public async Task<AuthServiceResponseDto> SeedRoleAsync()
        {
            bool isOwnerRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isOwnerRolesExists && isAdminRolesExists && isUserRolesExists)
            {
                return new AuthServiceResponseDto() { IsSucceed = false, Message = "Role Seeding is Already done" };
            }

            // Seeding the roles to the db
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));

            return new AuthServiceResponseDto() { IsSucceed = false, Message = "Role Seeding Done Successfully" };
        }
    }
}
