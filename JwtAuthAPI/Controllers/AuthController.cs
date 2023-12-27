using JwtAuthAPI.Core.Dtos;
using JwtAuthAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("seed-roles")]
        public async Task<ActionResult<AuthServiceResponseDto>> SeedRoles()
        {
            var result = await _authService.SeedRoleAsync();

            return Ok(result);

              

        }


        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="register"> The <see cref="RegisterDto"> instance </param>f
        /// <returns name="ActionResult"> the <see cref="ActionResult"/>></returns>
        [HttpPost]
        [Route("register")] 
        public async Task<ActionResult<AuthServiceResponseDto>> Register([FromBody] RegisterDto register)
        {
            var registerResult = await _authService.RegisterAsync(register);

            if (registerResult.IsSucceed)
            {
                return Ok(registerResult);
            }

            return BadRequest(registerResult);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult.IsSucceed)
            {
                return Ok(loginResult);
            }

            return BadRequest(loginResult);
      
        }

        [HttpPost]
        [Route("make-admin")]
        public async Task<ActionResult> MakeAdmin([FromBody] UpdatePermissionDto update)
        {

            var makeAdminResult = await _authService.MakeAdminAsync(update);

            if (makeAdminResult.IsSucceed)
            {
                return Ok(makeAdminResult);
            }

            return BadRequest(makeAdminResult);
          
        }

        [HttpPost]
        [Route("make-owner")]
        public async Task<ActionResult> MakeOwner([FromBody] UpdatePermissionDto update)
        {
            var makeOwnerResult = await _authService.MakeOwnerAsync(update);

            if (makeOwnerResult.IsSucceed)
            {
                return Ok(makeOwnerResult);
            }

            return BadRequest(makeOwnerResult);
        }
    }
}
