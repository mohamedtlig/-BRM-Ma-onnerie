using BRM.Web.Interfaces;
using BRM.Web.Models;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BRM.Web.Services;

public class PdfService : IPdfService
{
    private readonly IConfiguration _configuration;

    public PdfService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public byte[] GenererRecapitulatifEstimation(QuoteRequest quoteRequest)
    {
        var companyName = _configuration["CompanyInfo:Name"] ?? "BRM";
        var companyEmail = _configuration["CompanyInfo:Email"] ?? "";
        var companyPhone = _configuration["CompanyInfo:Phone"] ?? "";
        var companyCity = _configuration["CompanyInfo:City"] ?? "";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().Text(companyName).FontSize(22).Bold();
                    col.Item().Text($"Maçonnerie générale — {companyCity}").FontSize(11).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Text($"Estimation de travaux n° {quoteRequest.NumeroDemande}").FontSize(16).Bold();

                    col.Item().Background(Colors.Amber.Lighten4).Padding(8).Text(text =>
                    {
                        text.Span("DOCUMENT ESTIMATIF – NON CONTRACTUEL").Bold().FontColor(Colors.Amber.Darken3);
                    });

                    col.Item().Text("Informations client").Bold().FontSize(13);
                    col.Item().Text($"{quoteRequest.Prenom} {quoteRequest.Nom}");
                    col.Item().Text($"{quoteRequest.AdresseChantier}, {quoteRequest.CodePostal} {quoteRequest.Ville}");
                    col.Item().Text($"Email : {quoteRequest.Email}");
                    col.Item().Text($"Téléphone : {quoteRequest.Telephone}");

                    col.Item().PaddingTop(8).Text("Description du projet").Bold().FontSize(13);
                    col.Item().Text($"Prestation : {quoteRequest.Service?.Nom ?? "Non précisé"}");
                    if (quoteRequest.Surface.HasValue)
                        col.Item().Text($"Surface approximative : {quoteRequest.Surface} m²");
                    if (quoteRequest.Quantite.HasValue)
                        col.Item().Text($"Quantité : {quoteRequest.Quantite}");
                    col.Item().Text($"Description : {quoteRequest.Description}");

                    col.Item().PaddingTop(8).Background(Colors.Grey.Lighten3).Padding(12).Column(estCol =>
                    {
                        estCol.Item().Text("Estimation indicative").Bold().FontSize(13);
                        estCol.Item().Text($"{quoteRequest.EstimationBasse:N0} € – {quoteRequest.EstimationHaute:N0} € TTC")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                    });

                    col.Item().PaddingTop(8).Text(
                        "Cette estimation est fournie à titre indicatif sur la base des informations renseignées. " +
                        "Elle ne constitue pas un devis contractuel. Le montant définitif sera établi après étude " +
                        "du projet et visite du chantier par BRM.")
                        .FontSize(9).Italic().FontColor(Colors.Grey.Darken2);

                    col.Item().Text("Le devis définitif sera établi après visite et analyse du chantier.")
                        .FontSize(9).Italic().FontColor(Colors.Grey.Darken2);

                    col.Item().PaddingTop(8).Text($"Document généré le {DateTime.Now:dd/MM/yyyy à HH:mm}").FontSize(9);
                });

                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    col.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text($"{companyName} — {companyEmail} — {companyPhone}").FontSize(8);
                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.DefaultTextStyle(s => s.FontSize(8));
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}
