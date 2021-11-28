using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Validators
{
    public class ImageExtensionAttribute : ValidationAttribute
    {
        private readonly string[] validExtensions;

        public ImageExtensionAttribute()
        {
            validExtensions = new string[] { "bmp", "png", "jpg", "jpeg", };
        }

        public override bool IsValid(object value)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                string imgext = Path.GetExtension(file.FileName).Substring(1);

                if (!validExtensions.Contains(imgext))                
                    return false;                
            }

            return true;
        }
    }
}
