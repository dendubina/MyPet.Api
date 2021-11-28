using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Validators
{
    public class MaxImageSizeMBAttribute : ValidationAttribute
    {

        private const double maxImageSize = 5; 

        public override bool IsValid(object value)
        {
            var file = value as IFormFile;

            if(file != null)
            {
                if (file.Length > 1048576 * maxImageSize)  // 1MB * maxImageSize        
                    return false;                
            }

            return true;
        }
    }
}
