using BRM.Web.Interfaces;

namespace BRM.Web.Services;

public class FileUploadService : IFileUploadService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private static readonly Dictionary<string, byte[]> MagicNumbers = new()
    {
        [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47 },
        [".webp"] = new byte[] { 0x52, 0x49, 0x46, 0x46 }
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 Mo

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> EnregistrerImageAsync(IFormFile fichier, string sousDossier)
    {
        if (fichier is null || fichier.Length == 0)
            throw new InvalidOperationException("Le fichier est vide.");

        if (fichier.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("Le fichier dépasse la taille maximale autorisée (5 Mo).");

        var extension = Path.GetExtension(fichier.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Type de fichier non autorisé. Formats acceptés : JPG, PNG, WEBP.");

        await using (var stream = fichier.OpenReadStream())
        {
            var header = new byte[4];
            var bytesRead = await stream.ReadAsync(header.AsMemory(0, 4));
            var expectedMagic = MagicNumbers[extension.ToLowerInvariant()];
            if (bytesRead < expectedMagic.Length || !header.Take(expectedMagic.Length).SequenceEqual(expectedMagic))
                throw new InvalidOperationException("Le contenu du fichier ne correspond pas à une image valide.");
        }

        // Nom de fichier entièrement régénéré : neutralise tout risque de path traversal ou de collision.
        var safeFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", sousDossier);
        Directory.CreateDirectory(uploadsRoot);

        var fullPath = Path.Combine(uploadsRoot, safeFileName);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await fichier.CopyToAsync(fileStream);
        }

        _logger.LogInformation("Image enregistrée : {Path}", fullPath);

        return $"/uploads/{sousDossier}/{safeFileName}";
    }
}
