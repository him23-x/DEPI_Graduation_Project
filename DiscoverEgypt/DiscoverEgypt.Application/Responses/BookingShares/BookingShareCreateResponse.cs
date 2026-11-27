using DiscoverEgypt.Application.Enums.BookingShares;

namespace DiscoverEgypt.Application.Responses.BookingShares
{
    public sealed record BookingShareCreateResponse(
        BookingShareCreateOutcome Outcome,
        string? Token,
        DateTime? ExpiresAtUtc);
}
