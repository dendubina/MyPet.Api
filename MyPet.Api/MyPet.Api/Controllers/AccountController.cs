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
            var result = await accountService.CreateUser(model.Email, model.UserName, model.Password);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {            
            var result = await accountService.SignIn(model.Email, model.Password);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([Required]string userId, [Required]string emailToken)
         {
            var result = await accountService.ConfirmEmail(userId, emailToken);

            return Ok(new
            {
                status = 200,
                confirmationResult = result,
            });

        }

        [HttpGet]
        public async Task<IActionResult> CheckToken([Required]string jwttoken)
        {
            var result = await accountService.CheckToken(jwttoken);
            return Ok(result);
        }       
    }
}
