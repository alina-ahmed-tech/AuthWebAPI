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
    public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await authService.LoginAsync(request);
            if(token is null )
                return BadRequest("Invalid username or password");

            return Ok(token);
        }

    }
}
