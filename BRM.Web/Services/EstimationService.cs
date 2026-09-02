using BRM.Web.Data;
using BRM.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Services;

public class EstimationService : IEstimationService
{
    private readonly ApplicationDbContext _context;

    public EstimationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EstimationResult> CalculerAsync(int serviceId, decimal? surface, decimal? quantite)
    {
        var pricing = await _context.ServicePricings
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ServiceId == serviceId);

        if (pricing is null)
            return new EstimationResult(0, 0);

        var unites = surface ?? quantite ?? 0;

        var montantCentral = pricing.PrixBase
            + (pricing.PrixParUnite * unites * pricing.CoefficientComplexite)
            + pricing.FraisFixes;

        var basse = Math.Round(montantCentral * (1 - pricing.MargeFourchette), 0);
        var haute = Math.Round(montantCentral * (1 + pricing.MargeFourchette), 0);

        return new EstimationResult(Math.Max(0, basse), Math.Max(0, haute));
    }
}
