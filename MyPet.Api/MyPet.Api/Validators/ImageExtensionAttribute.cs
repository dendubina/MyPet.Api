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
        private static readonly Dictionary<string, List<byte[]>> fileSignatures = new Dictionary<string, List<byte[]>> { 
            
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

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var file = value as IFormFile;
            var ext = Path.GetExtension(file.FileName).ToLower();
            

            if (fileSignatures.ContainsKey(ext))
            {

                using (var reader = new BinaryReader(file.OpenReadStream()))
                {
                    var signatures = fileSignatures[ext];

                    var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                    if (signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature)))
                        return true;                    
                }

            }           

            return false;              
        }
    }
}
