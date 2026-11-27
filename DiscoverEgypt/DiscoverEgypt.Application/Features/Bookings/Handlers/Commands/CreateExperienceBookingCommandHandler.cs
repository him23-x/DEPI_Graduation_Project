using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Services;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands
{
    public sealed class CreateExperienceBookingCommandHandler : IRequestHandler<CreateExperienceBookingCommand, int>
    {
        private readonly ExperienceBookingService _service;

        public CreateExperienceBookingCommandHandler(ExperienceBookingService service) => _service = service;

        public Task<int> Handle(CreateExperienceBookingCommand request, CancellationToken cancellationToken) =>
            _service.CreateAsync(request.UserId, request.Form, cancellationToken);
    }
}
