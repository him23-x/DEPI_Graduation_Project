namespace DiscoverEgypt.Application.Common;

/// <summary>Something the request refers to does not exist. The Web API turns it into HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
