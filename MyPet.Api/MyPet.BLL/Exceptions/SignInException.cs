using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Exceptions
{
    [Serializable]
    public class SignInException : Exception
    {
        public List<string> Errors { get; }

        public SignInException(string message) : base(message) { }
        public SignInException(string message, List<string> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
