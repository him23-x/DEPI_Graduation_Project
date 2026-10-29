
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Policies;

internal static class MediaFilePolicy
{
    public const long ImageLimitBytes = 10 * 1024 * 1024;
    public const long VideoLimitBytes = 100 * 1024 * 1024;
    public const int MaxImages = 8;
    public const int MaxVideos = 2;

    public static (MediaType Type, string ContentType, long MaxLength) GetFileInfo(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => (MediaType.Image, "image/jpeg", ImageLimitBytes),
            ".png" => (MediaType.Image, "image/png", ImageLimitBytes),
            ".webp" => (MediaType.Image, "image/webp", ImageLimitBytes),
            ".mp4" => (MediaType.Video, "video/mp4", VideoLimitBytes),
            _ => (default, string.Empty, 0L)
        };
    }

    public static async Task<bool> HasExpectedSignatureAsync(Stream stream, string extension, CancellationToken cancellationToken)
    {
        var originalPosition = stream.Position;
        var signature = new byte[12];
        var read = await stream.ReadAsync(signature.AsMemory(), cancellationToken);
        stream.Position = originalPosition;

        return extension switch
        {
            ".jpg" or ".jpeg" => read >= 3 && signature[0] == 0xff && signature[1] == 0xd8 && signature[2] == 0xff,
            ".png" => read >= 8 && signature.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }),
            ".webp" => read >= 12 && signature.AsSpan(0, 4).SequenceEqual("RIFF"u8) && signature.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            ".mp4" => read >= 8 && signature.AsSpan(4, 4).SequenceEqual("ftyp"u8),
            _ => false
        };
    }
}

