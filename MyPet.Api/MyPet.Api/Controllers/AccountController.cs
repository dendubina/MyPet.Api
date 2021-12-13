using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyPet.Api.Models;
using MyPet.Api.Models.EmailModels;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.EmailModels;
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
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration config;
        private readonly EmailConfiguration emailConfig;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;


        public AccountController(SignInManager<IdentityUser> SignInManager, UserManager<IdentityUser> UserManager, IConfiguration config, IOptions<EmailConfiguration> options, IEmailService emailService, IMapper mapper)
        {
            this.SignInManager = SignInManager;
            userManager = UserManager;
            this.config = config;
            emailConfig = options.Value;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var createdUser = userManager.Users.SingleOrDefault(x => x.Email.Equals(model.Email));
                string code = await userManager.GenerateEmailConfirmationTokenAsync(createdUser);

                bool emailSendingResult = await emailService.SendConfirmationEmail(mapper.Map<EmailConfig>(emailConfig), createdUser.Email, createdUser.Id, code);

                return Ok(new { 
                    jwttoken = GenerateJWTToken(model.Email, createdUser),
                    isEmailSend = emailSendingResult,
                });;
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
                var user = userManager.Users.SingleOrDefault(x => x.Email.Equals(model.Email));                
                return Ok(new { jwttoken = GenerateJWTToken(user.Email, user) });
            }

            ModelState.AddModelError("error", "Неправильный логин или пароль");
            return ValidationProblem(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([Required]string userId, [Required]string emailToken)
         {
            var user = await userManager.FindByIdAsync(userId);

            if(user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, emailToken);

                if (result.Succeeded)
                {
                    return Ok(new {
                        status = 200,
                        confirmationResult = true,
                    });
                }
            }

            ModelState.AddModelError("error", "Something went wrong");
            return ValidationProblem(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> CheckToken([Required] string jwttoken)
        {            

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwttoken);
                Dictionary<string, string> claims = new Dictionary<string, string>();

               
                string userId = token.Claims.Where(x => x.Type == "unique_name").FirstOrDefault().Value;
                bool isEmailConfirmed = await GetIsEmailConfirmed(userId);
                claims.Add("isEmailConfirmed", isEmailConfirmed.ToString().ToLower());


                bool isTokenValid = ValidateToken(jwttoken);
                claims.Add("tokenValidation",  isTokenValid.ToString().ToLower());
                

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
                new Claim("username", user.UserName),
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

        private async Task<bool> GetIsEmailConfirmed(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user != null && user.EmailConfirmed == true)
                return true;
            else
                return false;
        }
    }
}
