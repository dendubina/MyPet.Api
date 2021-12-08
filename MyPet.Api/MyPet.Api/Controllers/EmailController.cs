using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyPet.Api.Models.EmailModels;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.EmailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private readonly IConfiguration config;
        private readonly EmailConfiguration emailConfig;
        private readonly IEmailService userService;
        private readonly IMapper mapper;


        public EmailController(IConfiguration config, IOptions<EmailConfiguration> options, IEmailService userService, IMapper mapper)
        {
            this.config = config;
            this.userService = userService;
            this.mapper = mapper;
            emailConfig = options.Value;
        }


        /*[HttpPost]
        public async Task<IActionResult> SendEmail(ConfirmEmailModel model)
        {
            bool result = await userService.SendConfirmationEmail(mapper.Map<EmailConfig>(emailConfig), model.ClientEmail);

            if (result)
                return Ok();
            else
                return BadRequest();
        }*/
    }
}
