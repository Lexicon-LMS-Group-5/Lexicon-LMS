using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Exceptions;

public class CourseCreationException : BadRequestException
{
    public CourseCreationException() : base("Could not create course")
    {

    }
    public CourseCreationException(string message) : base(message)
    {

    }
}
