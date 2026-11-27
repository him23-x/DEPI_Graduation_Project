using DiscoverEgypt.Application.Features.Reviews.Requests.Commands;
using DiscoverEgypt.Application.Features.Reviews.Services;
using DiscoverEgypt.Application.Responses.Reviews;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.Reviews.Handlers.Commands
{
    public sealed class CreateTouristReviewCommandHandler
        : IRequestHandler<CreateTouristReviewCommand, TouristReviewCreateResponse>
    {
        private readonly TouristReviewService _service;
        private readonly IValidator<CreateTouristReviewCommand> _validator;

        public CreateTouristReviewCommandHandler(
            TouristReviewService service,
            IValidator<CreateTouristReviewCommand> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<TouristReviewCreateResponse> Handle(
            CreateTouristReviewCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _service.CreateAsync(
                request.UserId,
                request.Form.BookingId,
                request.Form.Rating,
                request.Form.Comment,
                cancellationToken);
        }
    }
}
