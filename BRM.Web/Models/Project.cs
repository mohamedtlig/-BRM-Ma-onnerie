using System.ComponentModel.DataAnnotations;

namespace BRM.Web.Models;

/// <summary>Une réalisation présentée dans la galerie publique.</summary>
public class Project
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Titre { get; set; } = string.Empty;

    [Required, StringLength(3000)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string TypeTravaux { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Ville { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [StringLength(300)]
    public string? PhotoPrincipale { get; set; }

    public bool EstAvantApres { get; set; }

    public bool Publie { get; set; } = true;

    public ICollection<ProjectPhoto> ProjectPhotos { get; set; } = new List<ProjectPhoto>();
}
