using FluentValidation;
using Microsoft.AspNetCore.Http;
using MyPet.BLL.Models.Ads;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyPet.Api.Validators
{
    public class AdvertisementModelValidator : AbstractValidator<AdvertisementModel>
    {
        private static readonly string[] _validRegions = { "Brest", "Gomel", "Minsk", "Grodno", "Mogilev", "Vitebsk" };
        private static readonly string[] _validCategories = { "Lost", "Found" };

        public AdvertisementModelValidator()
        {
            RuleFor(model => model.Image).Must(ValidImageExtension).WithMessage("Wrong image extension");
            RuleFor(model => model.Image).Must(ValidImageSize).WithMessage("Wrong image size");

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

        private bool ValidRegions(string region) => _validRegions.Contains(region);


        private bool ValidCategories(string category) => _validCategories.Contains(category);
        

        private bool ValidImageExtension(object value)
        {
            if (value == null)
                return true;

            var fileSignatures = new Dictionary<string, List<byte[]>> {

            {".png", new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
            } },
            {".bmp", new List<byte[]>
            {
                new byte[] { 0x42, 0x4D },
            } },
            {".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
            } },
            {".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
            } },
        };

            var file = value as IFormFile;
            var ext = Path.GetExtension(file.FileName).ToLower();


            if (!fileSignatures.ContainsKey(ext))
            {
                return false;
            }

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var signatures = fileSignatures[ext];

                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                if (signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature)))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ValidImageSize(object value)
        {
            const double maxImageSize = 5;

            if (value is not IFormFile file)
            {
                return true;
            }

            return !(file.Length > 1048576 * maxImageSize);
        }
    }
}
