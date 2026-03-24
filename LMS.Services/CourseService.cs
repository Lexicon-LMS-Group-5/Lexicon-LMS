using LMS.Shared.DTOs;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        public Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query)
        {
            throw new NotImplementedException();
        }
    }
}
