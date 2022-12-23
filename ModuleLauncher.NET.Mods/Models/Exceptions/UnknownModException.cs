namespace ModuleLauncher.NET.Mods.Models.Exceptions;

public class UnknownModException : Exception
{
    public UnknownModException()
    {
    }

    public UnknownModException(string message) : base(message)
    {
    }

    public UnknownModException(string message, Exception inner) : base(message, inner)
    {
    }
}