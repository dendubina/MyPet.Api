using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPet.Api.Models;
using MyPet.BLL.Exceptions;
using MyPet.BLL.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {            
            this.accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var result = await accountService.CreateUser(model.Email, model.UserName, model.Password);
                return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
            }
            catch(UserCreatingException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError("error", error);
                }
                return ValidationProblem(ModelState);
            }            
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var result = await accountService.SignIn(model.Email, model.Password);
                return new ObjectResult(result); 
            }
            catch (SignInException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError("error", error);
                }
                return ValidationProblem(ModelState);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([Required]string userId, [Required]string emailToken)
         {
            var result = await accountService.ConfirmEmail(userId, emailToken);

            if (result)
            {
                return Ok(new
                {
                    status = 200,
                    confirmationResult = true,
                });
            }
            else
            {
                ModelState.AddModelError("error", "Something went wrong");
                return ValidationProblem(ModelState);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> CheckToken([Required]string jwttoken)
        {            

            try
            {
                var result = await accountService.CheckToken(jwttoken);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Unauthorized($"{ex.ParamName} {ex.Message}");
            }
        }       
    }
}
