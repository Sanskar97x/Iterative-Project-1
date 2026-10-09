using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.Exceptions
{
    public class UsernameAlreadyExistException : Exception
    {
        public UsernameAlreadyExistException() : base("Username already exists.")
        {
            
        }
    }
}
