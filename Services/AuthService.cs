using AuthWebAPI.Models;
using AuthWebAPI.Entities;
using AuthWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebAPI.Services
{
    public class AuthService(UserDbContext context, IConfiguration configuration) : IAuthService 
    {
        
        public async Task<string?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return null;
            }
            
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateToken(user);
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            // if user already exists (we are checking the users table in the code below in the database) then dont register them again, return null
            if(await context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return null;
            }

            var user = new User();
            //built in method from Microsoft Identity to hash our passwords
            var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password); //returns hashed version of the users password

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user); //making a change and EF is tracking it, buts its not actually been made yet - like git add
            await context.SaveChangesAsync(); //actaully makes/commits the changes - like git commit 

            return user;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
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