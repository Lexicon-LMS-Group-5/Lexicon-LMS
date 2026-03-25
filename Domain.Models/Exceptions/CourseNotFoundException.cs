using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Exceptions
{
    public class CourseNotFoundException() : NotFoundException("Course not found")
    {
    }
}
