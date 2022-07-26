using System.Runtime.Serialization;

namespace ModuleLauncher.NET.Models.Exceptions;

[Serializable]
public class ErrorParsingLibraryException : Exception
{
    public ErrorParsingLibraryException()
    {
    }

    public ErrorParsingLibraryException(string message) : base(message)
    {
    }

    public ErrorParsingLibraryException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ErrorParsingLibraryException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}