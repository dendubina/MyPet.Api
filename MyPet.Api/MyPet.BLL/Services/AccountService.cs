using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyPet.BLL.DTO;
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

        private readonly ILogger<AccountService> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;        
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly EmailConfig _emailConfig;

        public AccountService(ILogger<AccountService> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration config, IEmailService emailService, IOptions<EmailConfig> emailoptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _config = config;
            _emailConfig = emailoptions.Value;
            _logger = logger;
        }


        public async Task<object> CreateUser(string email, string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = email,
            };
            
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {                
                var createdUser = _userManager.Users.SingleOrDefault(x => x.Email.Equals(email));
                await _userManager.AddToRoleAsync(createdUser, "user");

                string code = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser);
                bool emailSendingResult = await _emailService.SendConfirmationEmail(_emailConfig, createdUser.Email, createdUser.Id, code);

                _logger.LogInformation($"User with email '{createdUser.Email}' and UserName '{createdUser.UserName}' has been created. Email has been send: {emailSendingResult}");

                return new
                {
                    registrationResult = result.Succeeded,
                    isEmailSend = emailSendingResult,
                };
            }

            var errorsDict = new Dictionary<string, string[]> { { "email, password, username", result.Errors.Select(error => error.Description).ToArray() } };

            _logger.LogWarning($"User with email '{email}' and UserName '{username}' has NOT been created. Reason: {result.Errors.First().Description}");

            throw new ValidationException("invalid email or password", errorsDict);
        }

        public async Task<UserProfileDTO> SignIn(string email, string password)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email.Equals(email));

            if (user == null)
            {
                throw new ValidationException("wrong email or password", new Dictionary<string, string[]> { { "email or password", new [] { "wrong email or password" } } });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (result.Succeeded)
            {
                return await GetTokenData(await GenerateJWTToken(user), user);
            }

            throw new ValidationException("wrong email or password", new Dictionary<string, string[]> { { "email or password", new [] { "wrong email or password" } } });
        }

        public async Task<bool> ConfirmEmail(string userId, string emailToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, emailToken);

                if (result.Succeeded)
                {
                    return true;
                }

                _logger.LogError($"User with Id '{userId}' could not confirm his email");
                throw new Exception($"User with Id '{userId}' could not confirm his email");
            }

            _logger.LogError($"User with Id '{userId}' was not found confirming his email");
            throw new UnauthorizedAccessException("User not found");
        }

        public async Task<UserProfileDTO> CheckToken(string jwttoken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwttoken);
                string userId = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
                var user = await _userManager.FindByIdAsync(userId);                               

                return await GetTokenData(jwttoken, user);
            }
            catch
            {
                throw new UnauthorizedAccessException("JWT token is not valid");
            }
        }

        private async Task<string> GenerateJWTToken(IdentityUser user)
        {
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.UniqueName, user.Id),
                new("username", user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(Convert.ToInt32(_config["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _config["JwtIssuer"],
                _config["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserProfileDTO> GetTokenData(string token, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesList = userRoles.ToList();

            return new UserProfileDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                TokenValidation = ValidateToken(token),
                JwtToken = token,
                Roles = rolesList,
            };
        }

        private bool ValidateToken(string token)
        {

            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
            var myIssuer = _config["JwtIssuer"];
            var myAudience = _config["JwtIssuer"];

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,

                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,

                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out _);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
