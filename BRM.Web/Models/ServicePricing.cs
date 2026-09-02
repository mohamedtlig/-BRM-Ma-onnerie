using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BRM.Web.Models;

/// <summary>
/// Paramètres de calcul d'estimation pour un service, modifiables par le gérant depuis l'admin.
/// Aucun de ces montants ne doit être codé en dur dans les contrôleurs.
/// </summary>
public class ServicePricing
{
    public int Id { get; set; }

    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 1000000)]
    public decimal PrixBase { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 100000)]
    public decimal PrixParUnite { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Range(0.1, 10)]
    public decimal CoefficientComplexite { get; set; } = 1.0m;

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 100000)]
    public decimal FraisFixes { get; set; }

    /// <summary>Marge appliquée pour construire la fourchette basse/haute autour de l'estimation centrale (ex : 0.15 = ±15 %).</summary>
    [Column(TypeName = "decimal(4,2)")]
    [Range(0, 1)]
    public decimal MargeFourchette { get; set; } = 0.15m;

    public DateTime DateMiseAJour { get; set; } = DateTime.UtcNow;
}
