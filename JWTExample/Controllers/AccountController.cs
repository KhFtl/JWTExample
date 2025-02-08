using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTSettings _jwtSettings;

        private List<User> Users = new List<User>
        {
            new User { Login = "admin", Password = "123" },
            new User { Login = "user", Password = "123" }
        };

        public AccountController(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] User user)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest("Invalid user data");
            }
            if (Users.FirstOrDefault(x => x.Login == user.Login && x.Password == user.Password) == null)
            {
                return NotFound("User not found or incorrect login/password");
            }
            else
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Login));
                claims.Add(new Claim(ClaimTypes.Gender, "Femail"));

                if (user.Login == "admin")
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    claims.Add(new Claim(ClaimTypes.GivenName, "Julie Doe Admin"));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.GivenName, "Natalie Shipshilova"));
                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                }
                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var jwt_token = new JwtSecurityToken(
                        issuer: _jwtSettings.Issure,
                        audience: _jwtSettings.Audience,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new JwtSecurityTokenHandler().WriteToken(jwt_token));
            }
        }
    }
}
