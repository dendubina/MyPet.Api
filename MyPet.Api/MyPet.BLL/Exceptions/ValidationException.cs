using System;
using System.Collections.Generic;

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
