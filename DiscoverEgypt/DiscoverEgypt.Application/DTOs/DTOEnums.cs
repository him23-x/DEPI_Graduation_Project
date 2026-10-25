namespace DiscoverEgypt.Application.DTOs.Bookings
{
    public enum HostBookingTab { Pending, Confirmed, Completed, RejectedCancelled }

    public enum HostBookingActionResult
    {
        Success, BookingUnavailable, NotPending, PaymentNotPaid, ResponseWindowExpired,
        ProviderSuspended, Overlap, ConcurrencyUnavailable, PayoutAlreadyExists,
        InvalidRefundAmount, NotConfirmed, NotEnded, OpenDispute
    }
}

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public enum EvidenceExperienceType { Unsupported, Food, Arts, LocalTraditions }
    public enum ExperienceVenueUse { PublicRoute, PrivateHome, ThirdPartyControlled }
    public enum ExperienceSubmissionResult { Success, ExperienceUnavailable, DeclarationNotAccepted, Incomplete, UnsupportedType }

    public enum ExperienceEvidenceMutationResult
    {
        Success, ExperienceUnavailable, EvidenceUnavailable, InvalidEvidenceKind,
        InvalidFile, FileLimitReached, TextRequired, TextTooLong
    }

    public enum ExperienceMediaMutationResult
    {
        Success, ExperienceUnavailable, ImageLimitReached, VideoLimitReached,
        InvalidFile, MediaUnavailable, CoverMustBeImage, InvalidOrder
    }
}

