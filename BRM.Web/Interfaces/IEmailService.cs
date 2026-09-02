using BRM.Web.Models;

namespace BRM.Web.Interfaces;

public interface IEmailService
{
    /// <summary>Envoie au client son estimation indicative, avec le PDF récapitulatif en pièce jointe.</summary>
    Task EnvoyerEstimationClientAsync(QuoteRequest quoteRequest, byte[] pdfRecapitulatif);

    /// <summary>Notifie le gérant qu'une nouvelle demande de devis a été déposée.</summary>
    Task EnvoyerNotificationGerantAsync(QuoteRequest quoteRequest);
}
