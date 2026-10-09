using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.Exceptions
{
    public class InvalidPreferredCalendarException : Exception
    {
        public InvalidPreferredCalendarException() : base("Preferred calendar must be either Englist or Nepali")
        { }
    }
}
