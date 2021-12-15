using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyPet.BLL.Exceptions;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.EmailModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Services
{
    public class AccountService : IAccountService
    {

        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmailService emailService;
        private readonly IConfiguration config;
        private readonly EmailConfig emailConfig;

        public AccountService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration config, IEmailService emailService, IOptions<EmailConfig> emailoptions)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;            
            this.config = config;
            emailConfig = emailoptions.Value;
        }


        public async Task<object> CreateUser(string email, string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = email,
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var createdUser = userManager.Users.SingleOrDefault(x => x.Email.Equals(email));
                string code = await userManager.GenerateEmailConfirmationTokenAsync(createdUser);

                bool emailSendingResult = await emailService.SendConfirmationEmail(emailConfig, createdUser.Email, createdUser.Id, code);

                return new {
                    jwttoken = GenerateJWTToken(createdUser),
                    isEmailSend = emailSendingResult,
                };
            }
            else
            {
                List<string> errorsList = new List<string>();

                foreach(var error in result.Errors)
                {
                    errorsList.Add(error.Description);
                }
                
                throw new UserCreatingException(errorsList.FirstOrDefault(), errorsList);
            }

        }

        public async Task<object> SignIn(string email, string password)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                var user = userManager.Users.SingleOrDefault(x => x.Email.Equals(email));

                return new
                {
                    jwttoken = GenerateJWTToken(user)
                };
            }
            else
                throw new SignInException("wrong email or password", new List<string> {"wrong email or password"});
        }

        public async Task<bool> ConfirmEmail(string userId, string emailToken)
        {
            var user = await userManager.FindByIdAsync(userId);

            if(user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, emailToken);

                if (result.Succeeded)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<Dictionary<string, string>> CheckToken(string jwttoken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwttoken);

                Dictionary<string, string> claims = new Dictionary<string, string>();
                foreach (var claim in token.Claims)
                {
                    claims.Add(claim.Type, claim.Value);
                }


                string userId = token.Claims.Where(x => x.Type == "unique_name").FirstOrDefault().Value;
                bool isEmailConfirmed = await GetIsEmailConfirmed(userId);
                claims.Add("isEmailConfirmed", isEmailConfirmed.ToString().ToLower());


                bool isTokenValid = ValidateToken(jwttoken);
                claims.Add("tokenValidation", isTokenValid.ToString().ToLower());
                               

                return claims;
            }
            catch
            {
                throw new ArgumentException("JWT token is not valid", "jwttoken");
            }
        }


        private string GenerateJWTToken(IdentityUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
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
