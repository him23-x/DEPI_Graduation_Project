namespace DiscoverEgypt.Infrastructure.Storage;

internal sealed class LocalExperienceFileStorage(string rootPath)
{
    private readonly string _rootPath = Path.GetFullPath(rootPath);

    public async Task UploadAsync(
        string storageKey,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        var path = ResolvePath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var file = new FileStream(
            path,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        await content.CopyToAsync(file, cancellationToken);
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Stream file = new FileStream(
            ResolvePath(storageKey),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Task.FromResult(file);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ResolvePath(storageKey);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    public static string GetContentType(string storageKey) => Path.GetExtension(storageKey).ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".mp4" => "video/mp4",
        ".pdf" => "application/pdf",
        _ => "application/octet-stream"
    };

    private string ResolvePath(string storageKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);

        var normalizedKey = storageKey.Replace('\\', '/');
        if (Path.IsPathRooted(storageKey))
            throw new InvalidOperationException("Storage keys must be relative paths.");

        var segments = normalizedKey.Split('/');
        if (segments.Any(segment =>
                string.IsNullOrWhiteSpace(segment)
                || segment is "." or ".."
                || segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
        {
            throw new InvalidOperationException("The storage key contains an invalid path segment.");
        }

        var path = Path.GetFullPath(Path.Combine(_rootPath, Path.Combine(segments)));
        var relativePath = Path.GetRelativePath(_rootPath, path);
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", comparison)
            || Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException("The storage key resolves outside the storage directory.");
        }

        return path;
    }
}
