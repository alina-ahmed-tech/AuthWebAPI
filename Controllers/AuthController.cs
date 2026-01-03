using AuthWebAPI.Models;
using AuthWebAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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
    }
}
