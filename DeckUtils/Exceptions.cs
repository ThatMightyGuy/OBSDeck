namespace DeckUtils;
public class InvalidResultException : ArgumentException
{
    public InvalidResultException(string? source, string? expected, string? got) : base($"Expected {expected}, got {got}", source) {}
}
public class InterfaceNotImplementedException : NotImplementedException
{
    public InterfaceNotImplementedException(string? message) : base(message) {}
    public InterfaceNotImplementedException(string? message, Exception? inner) : base(message, inner) {}

}