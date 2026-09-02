using BRM.Web.Interfaces;
using BRM.Web.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BRM.Web.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task EnvoyerEstimationClientAsync(QuoteRequest quoteRequest, byte[] pdfRecapitulatif)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress($"{quoteRequest.Prenom} {quoteRequest.Nom}", quoteRequest.Email));
        message.Subject = "Votre estimation de travaux – BRM";

        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                <p>Bonjour {quoteRequest.Prenom},</p>
                <p>Nous vous remercions pour votre demande de devis n° <strong>{quoteRequest.NumeroDemande}</strong>.</p>
                <p><strong>Résumé de votre demande :</strong><br/>
                Prestation : {quoteRequest.Service?.Nom}<br/>
                Description : {quoteRequest.Description}</p>
                <p><strong>Estimation indicative :</strong><br/>
                Entre {quoteRequest.EstimationBasse:N0} € et {quoteRequest.EstimationHaute:N0} € TTC</p>
                <p style="color:#a15c00;background:#fff3cd;padding:10px;border-radius:4px;">
                Cette estimation est fournie à titre indicatif sur la base des informations renseignées.
                Elle ne constitue pas un devis contractuel. Le montant définitif sera établi après étude
                du projet et visite du chantier par BRM.</p>
                <p>Nous reviendrons vers vous prochainement afin d'organiser une visite du chantier.</p>
                <p>Coordonnées BRM :<br/>{_settings.SenderEmail}</p>
                """,
            TextBody = $"Bonjour {quoteRequest.Prenom},\n\n" +
                       $"Estimation indicative pour la demande {quoteRequest.NumeroDemande} : " +
                       $"entre {quoteRequest.EstimationBasse:N0} € et {quoteRequest.EstimationHaute:N0} € TTC.\n" +
                       "Cette estimation n'est pas un devis contractuel. Le montant définitif sera établi après " +
                       "visite du chantier.\n\nBRM"
        };

        builder.Attachments.Add($"Estimation-{quoteRequest.NumeroDemande}.pdf", pdfRecapitulatif, new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        await EnvoyerAsync(message);
    }

    public async Task EnvoyerNotificationGerantAsync(QuoteRequest quoteRequest)
    {
        if (string.IsNullOrWhiteSpace(_settings.ManagerEmail))
        {
            _logger.LogWarning("Aucune adresse gérant configurée : notification non envoyée pour {Numero}.", quoteRequest.NumeroDemande);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(_settings.ManagerEmail));
        message.Subject = $"Nouvelle demande de devis – {quoteRequest.NumeroDemande}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                <p>Nouvelle demande de devis reçue.</p>
                <ul>
                  <li>Client : {quoteRequest.Prenom} {quoteRequest.Nom}</li>
                  <li>Téléphone : {quoteRequest.Telephone}</li>
                  <li>Email : {quoteRequest.Email}</li>
                  <li>Adresse chantier : {quoteRequest.AdresseChantier}, {quoteRequest.CodePostal} {quoteRequest.Ville}</li>
                  <li>Prestation : {quoteRequest.Service?.Nom}</li>
                  <li>Description : {quoteRequest.Description}</li>
                  <li>Estimation calculée : {quoteRequest.EstimationBasse:N0} € – {quoteRequest.EstimationHaute:N0} € TTC</li>
                  <li>Photos jointes : {quoteRequest.QuotePhotos.Count}</li>
                  <li>Date : {quoteRequest.DateCreation:dd/MM/yyyy HH:mm}</li>
                </ul>
                <p>Ouvrir la demande dans l'administration : /Admin/QuoteRequests/Details/{quoteRequest.Id}</p>
                """
        };
        message.Body = builder.ToMessageBody();

        await EnvoyerAsync(message);
    }

    private async Task EnvoyerAsync(MimeMessage message)
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
        {
            _logger.LogWarning("SMTP non configuré : e-mail '{Subject}' non envoyé.", message.Subject);
            return;
        }

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort,
                _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            if (!string.IsNullOrWhiteSpace(_settings.SmtpUser))
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Échec de l'envoi de l'e-mail '{Subject}'.", message.Subject);
            throw;
        }
    }
}
