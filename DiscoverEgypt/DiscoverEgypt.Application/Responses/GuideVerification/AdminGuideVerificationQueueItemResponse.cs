namespace DiscoverEgypt.Application.Responses.GuideVerification
{
    public sealed record AdminGuideVerificationQueueItemResponse(
        int VerificationRequestId,
        int GuideProfileId,
        string GuideName,
        DateOnly LicenseExpiryDate,
        DateTime SubmittedAt,
        int DocumentCount);
}
