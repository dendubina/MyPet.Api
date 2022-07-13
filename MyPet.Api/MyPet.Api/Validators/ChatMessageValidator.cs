using FluentValidation;
using MyPet.Api.SignalRHub.Models;

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
