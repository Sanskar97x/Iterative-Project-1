using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.Exceptions
{
    public class EmailAlreadyExistsException : Exception //Creating Custom Exception
    {
        public EmailAlreadyExistsException() : base("Email already exists")
        { 
        
        }
    }
}
