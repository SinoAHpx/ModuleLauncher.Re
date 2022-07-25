using System.Runtime.Serialization;

namespace ModuleLauncher.NET.Models.Exceptions;

[Serializable]
public class FailedAuthenticationException : Exception
{
    public FailedAuthenticationException()
    {
    }

    public FailedAuthenticationException(string message) : base(message)
    {
    }

    public FailedAuthenticationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected FailedAuthenticationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}