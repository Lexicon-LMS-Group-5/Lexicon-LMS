namespace Domain.Models.Exceptions;

public class UserNotFoundException(string message) : NotFoundException(message)
{
}
