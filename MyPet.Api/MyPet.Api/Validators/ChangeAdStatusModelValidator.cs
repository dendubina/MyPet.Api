using FluentValidation;
using MyPet.Api.Models;
using System.Linq;

namespace MyPet.Api.Validators
{
    public class ChangeAdStatusModelValidator : AbstractValidator<ChangeAdStatusModel>
    {
        private readonly string[] _validStatuses = { "Approved", "Rejected", "OnModeration" };

        public ChangeAdStatusModelValidator()
        {
            RuleFor(x => x.AdId).NotNull().NotEqual(0);

            RuleFor(x => x.Status).NotNull().NotEmpty();
            RuleFor(x => x.Status).Must(ValidStatuses).WithMessage("'Approved', 'Rejected' or 'OnModeration' status is required");

        }

        private bool ValidStatuses(string status) => _validStatuses.Contains(status);
    }
}
