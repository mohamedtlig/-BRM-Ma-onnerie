using BRM.Web.Data;
using BRM.Web.Interfaces;
using BRM.Web.Models;
using BRM.Web.ViewModels.Quote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Controllers;

public class QuoteController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEstimationService _estimationService;
    private readonly IEmailService _emailService;
    private readonly IPdfService _pdfService;
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<QuoteController> _logger;

    public QuoteController(
        ApplicationDbContext context,
        IEstimationService estimationService,
        IEmailService emailService,
        IPdfService pdfService,
        IFileUploadService fileUploadService,
        ILogger<QuoteController> logger)
    {
        _context = context;
        _estimationService = estimationService;
        _emailService = emailService;
        _pdfService = pdfService;
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? serviceId)
    {
        ViewData["Title"] = "Demander un devis — BRM Maçonnerie";
        ViewData["MetaDescription"] = "Décrivez votre projet de maçonnerie et recevez une estimation indicative gratuite. Devis définitif après visite du chantier par BRM.";

        var model = new QuoteRequestCreateViewModel
        {
            ServicesDisponibles = await GetServicesSelectListAsync(),
            ServiceId = serviceId ?? 0
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuoteRequestCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ServicesDisponibles = await GetServicesSelectListAsync();
            return View(model);
        }

        var serviceExists = await _context.Services.AnyAsync(s => s.Id == model.ServiceId && s.Actif);
        if (!serviceExists)
        {
            ModelState.AddModelError(nameof(model.ServiceId), "Le service sélectionné n'est plus disponible.");
            model.ServicesDisponibles = await GetServicesSelectListAsync();
            return View(model);
        }

        var estimation = await _estimationService.CalculerAsync(model.ServiceId, model.Surface, model.Quantite);

        var quoteRequest = new QuoteRequest
        {
            Prenom = model.Prenom.Trim(),
            Nom = model.Nom.Trim(),
            Email = model.Email.Trim(),
            Telephone = model.Telephone.Trim(),
            AdresseChantier = model.AdresseChantier.Trim(),
            CodePostal = model.CodePostal.Trim(),
            Ville = model.Ville.Trim(),
            ServiceId = model.ServiceId,
            Description = model.Description.Trim(),
            Surface = model.Surface,
            Quantite = model.Quantite,
            EtatActuel = model.EtatActuel,
            NiveauFinition = model.NiveauFinition,
            DelaiSouhaite = model.DelaiSouhaite,
            BudgetApproximatif = model.BudgetApproximatif,
            ConsentementRGPD = model.ConsentementRGPD,
            EstimationBasse = estimation.Basse,
            EstimationHaute = estimation.Haute
        };

        _context.QuoteRequests.Add(quoteRequest);
        await _context.SaveChangesAsync();

        quoteRequest.NumeroDemande = $"BRM-{quoteRequest.DateCreation:yyyy}-{quoteRequest.Id:D6}";

        if (model.Photos is not null)
        {
            foreach (var photo in model.Photos.Where(p => p.Length > 0).Take(8))
            {
                try
                {
                    var path = await _fileUploadService.EnregistrerImageAsync(photo, "quotes");
                    quoteRequest.QuotePhotos.Add(new QuotePhoto { CheminFichier = path });
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Photo rejetée pour la demande {Numero}.", quoteRequest.NumeroDemande);
                }
            }
        }

        await _context.SaveChangesAsync();

        await _context.Entry(quoteRequest).Reference(q => q.Service).LoadAsync();

        var emailEnvoye = await EnvoyerEmailsAsync(quoteRequest);

        return RedirectToAction(nameof(Result), new { id = quoteRequest.Id, sent = emailEnvoye });
    }

    [HttpGet]
    public async Task<IActionResult> Result(int id, bool sent = false)
    {
        var quoteRequest = await _context.QuoteRequests
            .Include(q => q.Service)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quoteRequest is null)
            return NotFound();

        ViewData["Title"] = $"Votre demande {quoteRequest.NumeroDemande} — BRM";

        var model = new QuoteResultViewModel
        {
            QuoteRequestId = quoteRequest.Id,
            NumeroDemande = quoteRequest.NumeroDemande,
            DateCreation = quoteRequest.DateCreation,
            Prenom = quoteRequest.Prenom,
            Nom = quoteRequest.Nom,
            Email = quoteRequest.Email,
            Telephone = quoteRequest.Telephone,
            AdresseChantier = quoteRequest.AdresseChantier,
            CodePostal = quoteRequest.CodePostal,
            Ville = quoteRequest.Ville,
            TypeTravaux = quoteRequest.Service?.Nom ?? "",
            Description = quoteRequest.Description,
            Surface = quoteRequest.Surface,
            Quantite = quoteRequest.Quantite,
            EstimationBasse = quoteRequest.EstimationBasse,
            EstimationHaute = quoteRequest.EstimationHaute,
            EmailEnvoye = sent
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenvoyerEmail(int id)
    {
        var quoteRequest = await _context.QuoteRequests
            .Include(q => q.Service)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quoteRequest is null)
            return NotFound();

        var sent = await EnvoyerEmailsAsync(quoteRequest, notifierGerant: false);

        TempData[sent ? "QuoteEmailSuccess" : "QuoteEmailError"] = sent
            ? "Votre estimation vient de vous être renvoyée par e-mail."
            : "L'envoi de l'e-mail a échoué. Merci de réessayer plus tard ou de nous contacter directement.";

        return RedirectToAction(nameof(Result), new { id });
    }

    private async Task<bool> EnvoyerEmailsAsync(QuoteRequest quoteRequest, bool notifierGerant = true)
    {
        try
        {
            var pdf = _pdfService.GenererRecapitulatifEstimation(quoteRequest);
            await _emailService.EnvoyerEstimationClientAsync(quoteRequest, pdf);

            if (notifierGerant)
                await _emailService.EnvoyerNotificationGerantAsync(quoteRequest);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Échec de l'envoi des e-mails pour la demande {Numero}.", quoteRequest.NumeroDemande);
            return false;
        }
    }

    private async Task<List<SelectListItem>> GetServicesSelectListAsync()
    {
        return await _context.Services
            .Where(s => s.Actif)
            .OrderBy(s => s.Ordre)
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nom })
            .ToListAsync();
    }
}
