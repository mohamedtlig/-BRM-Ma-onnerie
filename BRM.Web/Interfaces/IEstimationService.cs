using BRM.Web.Models;

namespace BRM.Web.Interfaces;

public record EstimationResult(decimal Basse, decimal Haute);

public interface IEstimationService
{
    /// <summary>
    /// Calcule une fourchette d'estimation indicative à partir des paramètres tarifaires
    /// configurés en base (ServicePricing) — jamais de montant codé en dur.
    /// </summary>
    Task<EstimationResult> CalculerAsync(int serviceId, decimal? surface, decimal? quantite);
}
