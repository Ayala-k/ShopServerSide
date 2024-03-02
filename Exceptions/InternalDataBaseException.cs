namespace serverSide.Exceptions;

public class InternalDataBaseException : Exception
{
    public InternalDataBaseException() : base() { }

    public InternalDataBaseException(string message) : base(message) { }

    public InternalDataBaseException(string message, Exception innerException) : base(message, innerException) { }
}
