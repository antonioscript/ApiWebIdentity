using ApiWebIdentity.Configuration;
using ApiWebIdentity.DTOs;
using ApiWebIdentity.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiWebIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        //private readonly JwtConfig _jwtConfig;
        public AuthenticationController(
            UserManager<IdentityUser> userManager,
           //JwtConfig jwtConfig
           IConfiguration configuration 
            )
        {
            _userManager = userManager;
            //_jwtConfig = jwtConfig;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestUser)
        {
            //Validate the incoming Request
            if (ModelState.IsValid)
            {
                //We need to check if the email already exist
                var userExist = await _userManager.FindByEmailAsync(requestUser.Email);

                if (userExist != null) 
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email Already Exist"
                        }
                    });
                }

                //Create a user
                var newUser = new IdentityUser()
                {
                    Email = requestUser.Email,
                    UserName = requestUser.Name
                };

                var isCreated = await _userManager.CreateAsync(newUser, requestUser.Password);  
                
                if (isCreated.Succeeded)
                {
                    //Generate the token
                    var token = GenerateJwtToken(newUser);
                    return Ok(new AuthResult()
                    {
                        Result = true, 
                        Token = token
                    });    
                }

                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                        {
                            "Server error"
                        }
                });

            }
            
            return BadRequest();
        }


        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestLogin)
        {
            if (ModelState.IsValid)
            {
                //Check if the user exist
                var existingUser = await _userManager.FindByEmailAsync(requestLogin.Email);

                if (existingUser == null) 
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid Payload"
                        }
                    });
                }

                var isCorrectPassword = await _userManager.CheckPasswordAsync(existingUser, requestLogin.Password);
                
                if (!isCorrectPassword)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid credentials"
                        }
                    });
                }

                var token = GenerateJwtToken(existingUser);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                        {
                            "Invalid Payload"
                        }
            });
        }

        private string GenerateJwtToken(IdentityUser user) 
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            //Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            { 
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
