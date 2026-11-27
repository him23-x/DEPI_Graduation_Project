namespace DiscoverEgypt.Application.Responses.GuideVerification
{
    public sealed record AdminGuideVerificationDetailsResponse(
        int VerificationRequestId,
        int GuideProfileId,
        string GuideName,
        string Bio,
        decimal HourlyRate,
        DateOnly LicenseExpiryDate,
        DateTime SubmittedAt,
        IReadOnlyList<string> GuideLanguages,
        IReadOnlyList<string> CulturalAreas,
        IReadOnlyList<AdminGuideVerificationDocumentResponse> Documents);
}
