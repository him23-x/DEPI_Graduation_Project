using DiscoverEgypt.Application.Features.BookingShares.Requests.Commands;
using DiscoverEgypt.Application.Features.BookingShares.Services;
using DiscoverEgypt.Application.Responses.BookingShares;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.BookingShares.Handlers.Commands
{
    public sealed class CreateTouristBookingShareCommandHandler
        : IRequestHandler<CreateTouristBookingShareCommand, BookingShareCreateResponse>
    {
        private readonly BookingShareService _service;
        private readonly IValidator<CreateTouristBookingShareCommand> _validator;

        public CreateTouristBookingShareCommandHandler(
            BookingShareService service,
            IValidator<CreateTouristBookingShareCommand> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<BookingShareCreateResponse> Handle(
            CreateTouristBookingShareCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _service.CreateAsync(request.UserId, request.BookingId, cancellationToken);
        }
    }
}
