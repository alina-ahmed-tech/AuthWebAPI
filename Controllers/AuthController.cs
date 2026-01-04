using AuthWebAPI.Models;
using AuthWebAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AuthWebAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            
            var user = await authService.RegisterAsync(request);
            if(user is null )
                return BadRequest("Username already exists.");
                
            return Ok(user);
        }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if(result is null )
                return BadRequest("Invalid username or password");

            return Ok(result);
        }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if(result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated, you can access this.");
        }    

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are authenticated, you can access this.");
        }    

    }
}
