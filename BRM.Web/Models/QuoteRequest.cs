using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BRM.Web.Models.Enums;

namespace BRM.Web.Models;

public class QuoteRequest
{
    public int Id { get; set; }

    /// <summary>Numéro lisible communiqué au client (ex : BRM-2026-000123).</summary>
    [StringLength(30)]
    public string NumeroDemande { get; set; } = string.Empty;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public QuoteStatus Statut { get; set; } = QuoteStatus.Nouvelle;

    // Informations client
    [Required, StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(20)]
    public string Telephone { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string AdresseChantier { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string CodePostal { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Ville { get; set; } = string.Empty;

    // Informations projet
    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    [Required, StringLength(3000)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 100000)]
    public decimal? Surface { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 100000)]
    public decimal? Quantite { get; set; }

    public EtatActuel EtatActuel { get; set; } = EtatActuel.Autre;

    public NiveauFinition NiveauFinition { get; set; } = NiveauFinition.NonDefini;

    [StringLength(100)]
    public string? DelaiSouhaite { get; set; }

    [StringLength(100)]
    public string? BudgetApproximatif { get; set; }

    // Estimation calculée (voir IEstimationService)
    [Column(TypeName = "decimal(10,2)")]
    public decimal EstimationBasse { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal EstimationHaute { get; set; }

    [Required]
    public bool ConsentementRGPD { get; set; }

    public ICollection<QuotePhoto> QuotePhotos { get; set; } = new List<QuotePhoto>();

    public ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();
}
