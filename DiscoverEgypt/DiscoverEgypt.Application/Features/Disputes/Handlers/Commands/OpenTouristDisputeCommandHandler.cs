using DiscoverEgypt.Application.Features.Disputes.Requests.Commands;
using DiscoverEgypt.Application.Features.Disputes.Services;
using DiscoverEgypt.Application.Responses.Disputes;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Commands
{
    public sealed class OpenTouristDisputeCommandHandler
        : IRequestHandler<OpenTouristDisputeCommand, TouristDisputeOpenResponse>
    {
        private readonly TouristDisputeService _service;
        private readonly IValidator<OpenTouristDisputeCommand> _validator;

        public OpenTouristDisputeCommandHandler(
            TouristDisputeService service,
            IValidator<OpenTouristDisputeCommand> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<TouristDisputeOpenResponse> Handle(
            OpenTouristDisputeCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _service.OpenAsync(request.UserId, request.Form.BookingId, request.Form.Description, cancellationToken);
        }
    }
}
