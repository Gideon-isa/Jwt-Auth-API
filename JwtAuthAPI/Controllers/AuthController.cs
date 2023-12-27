using JwtAuthAPI.Core.Dtos;
using JwtAuthAPI.Core.Entities;
using JwtAuthAPI.Core.JwtProvider;
using JwtAuthAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtProvider _jwtProvider;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _jwtProvider = jwtProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("seed-roles")]
        public async Task<ActionResult> SeedRoles()
        {
            bool isOwnerRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isOwnerRolesExists && isAdminRolesExists && isUserRolesExists)
            {
                return Ok("Role Seeding is Already done");
            }

            // Seeding the roles to the db
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));

            return Ok("Role Seeding Done Successfully");


        }


        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="register"> The <see cref="RegisterDto"> instance </param>f
        /// <returns name="ActionResult"> the <see cref="ActionResult"/>></returns>
        [HttpPost]
        [Route("register")] 
        public async Task<ActionResult> Register([FromBody] RegisterDto register)
        {
            var isExistsUser = await _userManager.FindByNameAsync(register.UserName);

            if (isExistsUser != null)
            {
                return BadRequest("Username already exists");
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
                return BadRequest(errorString);
            }

            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            return Ok("User Created Successfully");


        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            //gets the user by username
            ApplicationUser? user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
            {
                return Unauthorized("Invalid Credentials");
            }

            bool isPasswordCorrect =  await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect) 
            {
                return Unauthorized("Invalid Credentials");

            }

            var userRoles = (List<string>) await _userManager.GetRolesAsync(user);

            string token = _jwtProvider.GenerateToken(user, userRoles);

            return Ok(token);
        }

        [HttpPost]
        [Route("make-admin")]
        public async Task<ActionResult> MakeAdmin([FromBody] UpdatePermissionDto update)
        {
            var user = await _userManager.FindByNameAsync(update.UserName);

            if (user is null)
            {
                return BadRequest("Invalid user");
            }

            var taskResult = await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return Ok("User is now and Admin");
        }

        [HttpPost]
        [Route("make-owner")]
        public async Task<ActionResult> MakeOwner([FromBody] UpdatePermissionDto update)
        {
            var user = await _userManager.FindByNameAsync(update.UserName);

            if (user is null)
            {
                return BadRequest("Invalid user");
            }

            var taskResult = await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return Ok("User is now and Owner");
        }
    }
}
