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
        
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {            
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _accountService.CreateUser(model.Email, model.UserName, model.Password);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {            
            var result = await _accountService.SignIn(model.Email, model.Password);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([Required]string userId, [Required]string emailToken)
         {
            var result = await _accountService.ConfirmEmail(userId, emailToken);

            return Ok(new
            {
                status = 200,
                confirmationResult = result,
            });

        }

        [HttpPost]
        public async Task<IActionResult> CheckToken([Required]string jwttoken)
        {
            var result = await _accountService.CheckToken(jwttoken);
            return Ok(result);
        }       
    }
}
