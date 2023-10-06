namespace DeckUtils;
public class InvalidResultException : ArgumentException
{
    public InvalidResultException(string source, string expected, string got) : base($"Expected {expected}, got {got}", source) {}
}