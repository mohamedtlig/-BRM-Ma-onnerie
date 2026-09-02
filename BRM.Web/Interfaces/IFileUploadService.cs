namespace BRM.Web.Interfaces;

public interface IFileUploadService
{
    /// <summary>
    /// Valide (type MIME, extension, taille) puis enregistre un fichier image sous un nom sécurisé
    /// dans le sous-dossier donné de wwwroot/uploads. Retourne le chemin relatif stocké en base.
    /// Lève <see cref="InvalidOperationException"/> si le fichier ne respecte pas les contraintes.
    /// </summary>
    Task<string> EnregistrerImageAsync(IFormFile fichier, string sousDossier);
}
