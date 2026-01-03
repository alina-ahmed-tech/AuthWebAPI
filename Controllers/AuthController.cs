using AuthWebAPI.Models;
using AuthWebAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public static User user = new();

        [HttpPost("register")]
        //test to see the user - delete later 
        public ActionResult<User> Register(UserDto request)
        {
            //built in method from Microsoft Identity to hash our passwords
            var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password); //returns hashed version of the users password

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            return Ok(user);
        }

    [HttpPost("login")]
    public ActionResult<string> Login(UserDto request)
        {
            if(user.Username != request.Username)
            {
                return BadRequest("User not found");
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong password."); //delete later, for testing
            }
            var token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            // the jwt secret key to verify its a legit token, untampered
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); //because of the algorithm choice (HmacSha512), our token needs to be minimum 512 bits long (aka 64 char)

            // the body (aka payload) of the jwt
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"), //configuration values are in the appsettings.json 
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            // serialises the jwt
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); 
        }

    }
}
