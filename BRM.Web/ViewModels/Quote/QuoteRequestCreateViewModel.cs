using System.ComponentModel.DataAnnotations;
using BRM.Web.Models.Enums;
using BRM.Web.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BRM.Web.ViewModels.Quote;

public class QuoteRequestCreateViewModel
{
    // Informations client
    [Required(ErrorMessage = "Merci d'indiquer votre prénom.")]
    [StringLength(100)]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer votre nom.")]
    [StringLength(100)]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer votre e-mail.")]
    [EmailAddress(ErrorMessage = "Adresse e-mail invalide.")]
    [StringLength(200)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer votre téléphone.")]
    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    [StringLength(20)]
    [Display(Name = "Téléphone")]
    public string Telephone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer l'adresse du chantier.")]
    [StringLength(250)]
    [Display(Name = "Adresse du chantier")]
    public string AdresseChantier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer le code postal.")]
    [StringLength(10)]
    [Display(Name = "Code postal")]
    public string CodePostal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer la ville.")]
    [StringLength(100)]
    [Display(Name = "Ville")]
    public string Ville { get; set; } = string.Empty;

    // Informations projet
    [Required(ErrorMessage = "Merci de sélectionner un type de travaux.")]
    [Display(Name = "Type de travaux")]
    public int ServiceId { get; set; }

    public List<SelectListItem> ServicesDisponibles { get; set; } = new();

    [Required(ErrorMessage = "Merci de décrire votre projet.")]
    [StringLength(3000)]
    [Display(Name = "Description détaillée du projet")]
    public string Description { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Surface invalide.")]
    [Display(Name = "Surface approximative (m²)")]
    public decimal? Surface { get; set; }

    [Range(0, 100000, ErrorMessage = "Quantité invalide.")]
    [Display(Name = "Quantité")]
    public decimal? Quantite { get; set; }

    [Required(ErrorMessage = "Merci d'indiquer l'état actuel.")]
    [Display(Name = "État actuel")]
    public EtatActuel EtatActuel { get; set; } = EtatActuel.Autre;

    [Required(ErrorMessage = "Merci d'indiquer le niveau de finition souhaité.")]
    [Display(Name = "Niveau de finition souhaité")]
    public NiveauFinition NiveauFinition { get; set; } = NiveauFinition.NonDefini;

    [StringLength(100)]
    [Display(Name = "Délai souhaité")]
    public string? DelaiSouhaite { get; set; }

    [StringLength(100)]
    [Display(Name = "Budget approximatif")]
    public string? BudgetApproximatif { get; set; }

    [Display(Name = "Photos du chantier (facultatif)")]
    public List<IFormFile>? Photos { get; set; }

    [Required(ErrorMessage = "Vous devez accepter l'utilisation de vos données pour traiter votre demande.")]
    [MustBeTrue(ErrorMessage = "Vous devez accepter l'utilisation de vos données pour traiter votre demande.")]
    [Display(Name = "J'accepte que mes informations soient utilisées pour traiter ma demande de devis.")]
    public bool ConsentementRGPD { get; set; }
}
