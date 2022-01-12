using FluentValidation;
using MyPet.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Validators
{
    public class UpdatedAdvertisementModelValidator : AbstractValidator<UpdatedAdvertisementModel>
    {
        public UpdatedAdvertisementModelValidator()
        {
            RuleFor(model => model.PetName).NotNull().NotEmpty().MaximumLength(50);

            RuleFor(model => model.Category).NotNull().NotEmpty();
            RuleFor(model => model.Category).Must(ValidCategories).WithMessage("'Lost' or 'Found'");

            RuleFor(model => model.LocationRegion).Must(ValidRegions).WithMessage("'Brest', 'Gomel', 'Minsk', 'Grodno', 'Mogilev', 'Vitebsk'");

            RuleFor(model => model.LocationTown).NotNull().NotEmpty();
            RuleFor(model => model.LocationTown).Matches(@"^[0-9a-zA-ZЁёА-я ]+$").WithMessage("Only numbers and letters for town");
            RuleFor(model => model.LocationTown).MaximumLength(10).WithMessage("Less than 10 symbols for town");

            RuleFor(model => model.LocationStreet).NotNull().NotEmpty();
            RuleFor(model => model.LocationStreet).Matches(@"^[0-9a-zA-ZЁёА-я ]+$").WithMessage("Only numbers and letters for street");
            RuleFor(model => model.LocationStreet).MaximumLength(15).WithMessage("Less than 15 symbols for street");


            RuleFor(model => model.LocationHouse).NotNull().NotEmpty().MaximumLength(5);
            RuleFor(model => model.LocationHouse).Matches(@"^[0-9 ]+$").WithMessage("Only numbers for house");

            RuleFor(model => model.Description).NotNull().NotEmpty().MaximumLength(300);
        }

        private bool ValidRegions(string region)
        {
            string[] validRegions = { "Brest", "Gomel", "Minsk", "Grodno", "Mogilev", "Vitebsk" };

            if (validRegions.Contains(region))
                return true;
            else
                return false;
        }

        private bool ValidCategories(string category)
        {
            string[] validCategories = { "Lost", "Found"};

            if (validCategories.Contains(category))
                return true;
            else
                return false;
        } 
    }
}
