using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Exceptions
{
    [Serializable]
    public class UserCreatingException : Exception
    {
        public List<string> Errors { get;}

        public UserCreatingException(string message) : base(message) { }
        public UserCreatingException(string message, List<string> errors) : base(message)
        {
            Errors = errors;
        }


    }
}
