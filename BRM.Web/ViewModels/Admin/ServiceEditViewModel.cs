using System.ComponentModel.DataAnnotations;

namespace BRM.Web.ViewModels.Admin;

public class ServiceEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(150)]
    [Display(Name = "Nom du service")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est obligatoire.")]
    [StringLength(2000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Unité de facturation")]
    public string Unite { get; set; } = "m²";

    [Display(Name = "Service actif (visible sur le site)")]
    public bool Actif { get; set; } = true;

    [Display(Name = "Ordre d'affichage")]
    public int Ordre { get; set; }

    [Display(Name = "Photo du service")]
    public IFormFile? Image { get; set; }

    public string? ImagePath { get; set; }

    // Tarification (ServicePricing)
    [Display(Name = "Prix de base (€)")]
    [Range(0, 1000000)]
    public decimal PrixBase { get; set; }

    [Display(Name = "Prix par unité (€)")]
    [Range(0, 100000)]
    public decimal PrixParUnite { get; set; }

    [Display(Name = "Coefficient de complexité")]
    [Range(0.1, 10)]
    public decimal CoefficientComplexite { get; set; } = 1.0m;

    [Display(Name = "Frais fixes (€)")]
    [Range(0, 100000)]
    public decimal FraisFixes { get; set; }

    [Display(Name = "Marge de la fourchette (0.15 = ±15 %)")]
    [Range(0, 1)]
    public decimal MargeFourchette { get; set; } = 0.15m;
}
