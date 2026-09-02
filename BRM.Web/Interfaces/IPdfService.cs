using BRM.Web.Models;

namespace BRM.Web.Interfaces;

public interface IPdfService
{
    /// <summary>Génère le document PDF non contractuel récapitulant l'estimation d'une demande de devis.</summary>
    byte[] GenererRecapitulatifEstimation(QuoteRequest quoteRequest);
}
