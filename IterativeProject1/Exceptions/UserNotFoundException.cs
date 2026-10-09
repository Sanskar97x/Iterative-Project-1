using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.Exceptions
{
    public class UserNotFoundException : Exception 
    {
        public UserNotFoundException(string message) : base(message)
        {

        }
    }
}

/*This code creates a custom exception called UserNotFoundException. 
 * It inherits from C#’s built-in Exception class, so it works like a normal exception 
 * but specifically represents a situation where a user cannot be found. 
 * Its constructor accepts an error message, and base(message) passes that message to the parent Exception class so it can store and display it. 
 * This allows you to write something like throw new UserNotFoundException("User not found."); and clearly identify the type of error.*/