namespace Domain.Models.Exceptions
{
    public class CourseNotFoundException(int? courseId = 0)
        : NotFoundException($"Course(Id={courseId}) not found")
    { }
}
