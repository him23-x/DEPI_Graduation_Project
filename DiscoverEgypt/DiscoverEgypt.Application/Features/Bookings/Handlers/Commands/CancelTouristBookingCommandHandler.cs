using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Services;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands
{
    public sealed class CancelTouristBookingCommandHandler : IRequestHandler<CancelTouristBookingCommand, BookingCancellationOutcome>
    {
        private readonly TouristBookingService _service;
        public CancelTouristBookingCommandHandler(TouristBookingService service) => _service = service;
        public Task<BookingCancellationOutcome> Handle(CancelTouristBookingCommand request, CancellationToken cancellationToken) =>
            _service.CancelAsync(request.UserId, request.BookingId, cancellationToken);
    }
}
