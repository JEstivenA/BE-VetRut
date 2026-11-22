using BE_VetRuta.Utilitys;

namespace BE_VetRuta.Services;

public class ArchivosService(IWebHostEnvironment env)
{
    private static readonly string[] ExtensionesPermitidas = [".jpg", ".jpeg", ".png", ".webp"];
    private const long TamanoMaximoBytes = 5 * 1024 * 1024;

    public static string CarpetaUploads(IWebHostEnvironment env) =>
        Path.Combine(env.ContentRootPath, "uploads");

    public async Task<string> GuardarImagenAsync(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0)
            throw ApiException.BadRequest("Debe enviar una imagen.");

        if (archivo.Length > TamanoMaximoBytes)
            throw ApiException.BadRequest("La imagen no puede superar 5 MB.");

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (!ExtensionesPermitidas.Contains(extension))
            throw ApiException.BadRequest("Formato no permitido. Use JPG, PNG o WEBP.");

        var carpeta = CarpetaUploads(env);
        Directory.CreateDirectory(carpeta);

        var nombre = $"{Guid.NewGuid():N}{extension}";
        await using var stream = File.Create(Path.Combine(carpeta, nombre));
        await archivo.CopyToAsync(stream);

        return $"/uploads/{nombre}";
    }
}
