namespace DiscoverEgypt.Domain.Common;

/// <summary>A business rule was broken. The Web API turns it into HTTP 400.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
