using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Exceptions
{
    [Serializable]
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
