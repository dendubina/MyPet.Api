using FluentValidation;
using MyPet.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Validators
{
    public class ChangeAdStatusModelValidator : AbstractValidator<ChangeAdStatusModel>
    {
        public ChangeAdStatusModelValidator()
        {
            RuleFor(x => x.AdId).NotNull().NotEqual(0);

            RuleFor(x => x.Status).NotNull().NotEmpty();
            RuleFor(x => x.Status).Must(ValidStatuses).WithMessage("'Approved', 'Rejected' or 'OnModeration' status is required");

        }

        private bool ValidStatuses(string status)
        {
            string[] validStatuses = {"Approved", "Rejected", "OnModeration"};

            return validStatuses.Contains(status);
        }
    }
}
