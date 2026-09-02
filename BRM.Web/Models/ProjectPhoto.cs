using System.ComponentModel.DataAnnotations;

namespace BRM.Web.Models;

public class ProjectPhoto
{
    public int Id { get; set; }

    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    [Required, StringLength(300)]
    public string CheminFichier { get; set; } = string.Empty;

    /// <summary>Pour les projets "Avant/Après" : précise le rôle de la photo.</summary>
    public bool EstPhotoApres { get; set; }

    public int Ordre { get; set; }
}
