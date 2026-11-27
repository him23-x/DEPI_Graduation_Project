using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Services;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands
{
    public sealed class CreateGuideBookingCommandHandler : IRequestHandler<CreateGuideBookingCommand, int>
    {
        private readonly GuideBookingService _service;
        public CreateGuideBookingCommandHandler(GuideBookingService service) => _service = service;
        public Task<int> Handle(CreateGuideBookingCommand request, CancellationToken cancellationToken) =>
            _service.CreateAsync(request.UserId, request.Form, cancellationToken);
    }
}
