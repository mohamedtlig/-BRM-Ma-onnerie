using System.ComponentModel.DataAnnotations;

namespace BRM.Web.ViewModels.Admin;

public class ProjectEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est obligatoire.")]
    [StringLength(150)]
    [Display(Name = "Titre")]
    public string Titre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est obligatoire.")]
    [StringLength(3000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type de travaux est obligatoire.")]
    [StringLength(100)]
    [Display(Name = "Type de travaux")]
    public string TypeTravaux { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ville est obligatoire.")]
    [StringLength(100)]
    [Display(Name = "Ville")]
    public string Ville { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date de réalisation")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Display(Name = "Photo principale")]
    public IFormFile? PhotoPrincipale { get; set; }

    public string? PhotoPrincipaleExistante { get; set; }

    [Display(Name = "Photos supplémentaires")]
    public List<IFormFile>? PhotosSupplementaires { get; set; }

    [Display(Name = "Présentation Avant / Après")]
    public bool EstAvantApres { get; set; }

    [Display(Name = "Publié sur le site")]
    public bool Publie { get; set; } = true;
}
