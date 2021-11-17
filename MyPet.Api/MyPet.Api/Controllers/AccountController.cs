using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyPet.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> SignInManager;
        private readonly UserManager<IdentityUser> UserManager;
        private readonly IConfiguration config;


        public AccountController(SignInManager<IdentityUser> SignInManager, UserManager<IdentityUser> UserManager, IConfiguration config)
        {
            this.SignInManager = SignInManager;
            this.UserManager = UserManager;
            this.config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { jwttoken = GenerateJWTToken(model.Email, user) });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem(ModelState);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = UserManager.Users.SingleOrDefault(x => x.Email.Equals(model.Email));
                return Ok(new { jwttoken = GenerateJWTToken(user.Email, user) });
            }

            ModelState.AddModelError("error", "Неправильный логин или пароль");
            return ValidationProblem(ModelState);
        }

        [HttpGet]
        public IActionResult CheckToken([Required] string jwttoken)
        {

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwttoken);
                Dictionary<string, string> claims = new Dictionary<string, string>();

                claims.Add("tokenValidation", ValidateToken(jwttoken).ToString().ToLower());

                foreach (var claim in token.Claims)
                {
                    claims.Add(claim.Type, claim.Value);
                }

                return Ok(claims);
            }
            catch
            {
                ModelState.AddModelError("error", "JwtToken is invalid");
                return ValidationProblem(ModelState);
            }
        }


        private string GenerateJWTToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtKey"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(Convert.ToInt32(config["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                config["JwtIssuer"],
                config["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool ValidateToken(string token)
        {

            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtKey"]));
            var myIssuer = config["JwtIssuer"];
            var myAudience = config["JwtIssuer"];

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
