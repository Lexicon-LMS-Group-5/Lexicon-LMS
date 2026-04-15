namespace Domain.Models.Exceptions
{
    public class CourseNotFoundException(int? cid = 0)
        : NotFoundException($"Course(Id={cid}) not found")
    { }
}
