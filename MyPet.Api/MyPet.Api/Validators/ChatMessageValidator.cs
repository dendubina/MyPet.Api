using FluentValidation;
using MyPet.Api.SignalRHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Validators
{
    public class ChatMessageValidator : AbstractValidator<ChatMessage>
    {
        public ChatMessageValidator()
        {
            RuleFor(x => x.Message).NotNull().NotEmpty();
            RuleFor(x => x.Message).MaximumLength(300).WithMessage("300 symbols is maximum lenght");

            RuleFor(x => x.ToUserId).NotNull().NotEmpty();
        }
    }
}
