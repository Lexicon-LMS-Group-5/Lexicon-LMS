using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Exceptions
{
    public class CourseNotFoundException(int? cid = 0) 
        : NotFoundException($"Course(Id={cid}) not found") {}
}
