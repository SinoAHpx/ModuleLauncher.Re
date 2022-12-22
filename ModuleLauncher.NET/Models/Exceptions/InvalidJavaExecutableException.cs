namespace ModuleLauncher.NET.Models.Exceptions;

public class InvalidJavaExecutableException : Exception
{
    public InvalidJavaExecutableException()
    {
    }

    public InvalidJavaExecutableException(string message) : base(message)
    {
    }

    public InvalidJavaExecutableException(string message, Exception inner) : base(message, inner)
    {
    }
}