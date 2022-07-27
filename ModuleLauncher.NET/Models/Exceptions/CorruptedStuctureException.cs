using System.Runtime.Serialization;

namespace ModuleLauncher.NET.Models.Exceptions;

[Serializable]
public class CorruptedStuctureException : Exception
{
    public CorruptedStuctureException()
    {
    }

    public CorruptedStuctureException(string message) : base(message)
    {
    }

    public CorruptedStuctureException(string message, Exception inner) : base(message, inner)
    {
    }

    protected CorruptedStuctureException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}