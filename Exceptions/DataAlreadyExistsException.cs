namespace serverSide.Exceptions;

public class DataAlreadyExistsException : Exception
{
    public DataAlreadyExistsException() : base() { }

    public DataAlreadyExistsException(string message) : base(message) { }

    public DataAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
