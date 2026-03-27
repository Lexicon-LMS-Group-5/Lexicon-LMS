using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException() : base("User not found") 
        { 
        }

        public UserNotFoundException(string message) : base(message) 
        { 
        }
    }
}
