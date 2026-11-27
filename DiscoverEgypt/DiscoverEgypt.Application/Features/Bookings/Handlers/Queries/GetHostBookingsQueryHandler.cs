using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Application.Features.Bookings.Policies;
using MediatR;
using Microsoft.Extensions.Options;
using AutoMapper;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetHostBookingsQueryHandler : IRequestHandler<GetHostBookingsQuery, IReadOnlyList<HostBookingListItemResponse>>
    {
        private readonly IHostBookingRepository _repository;
        private readonly IOptions<BookingLifecycleOptions> _lifecycleOptions;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostBookingsQuery> _validator;

        public GetHostBookingsQueryHandler(
            IHostBookingRepository repository,
            IOptions<BookingLifecycleOptions> lifecycleOptions,
            IMapper mapper,
            IValidator<GetHostBookingsQuery> validator)
        {
            _repository = repository;
            _lifecycleOptions = lifecycleOptions;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IReadOnlyList<HostBookingListItemResponse>> Handle(GetHostBookingsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var bookings = await _repository.GetForHostAsync(request.UserId, request.Tab, cancellationToken);
            var tourists = await _repository.GetTouristsAsync(bookings.Select(booking => booking.TouristUserId), cancellationToken);
            return _mapper.Map<IReadOnlyList<HostBookingListItemResponse>>(bookings).Select((item, index) =>
            {
                var booking = bookings[index];
                var tourist = tourists.GetValueOrDefault(booking.TouristUserId);
                return item with
                {
                    TouristName = !string.IsNullOrWhiteSpace(tourist?.FullName) ? tourist.FullName : tourist?.UserName ?? "Discover Egypt guest",
                    PaidAt = booking.Payment.PaidAt,
                    ResponseDeadline = HostBookingPolicy.GetResponseDeadline(booking.Payment.PaidAt, _lifecycleOptions.Value.ProviderResponseTimeout)
                };
            }).ToArray();
        }
    }
}

