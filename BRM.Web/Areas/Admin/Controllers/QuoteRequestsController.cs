using BRM.Web.Data;
using BRM.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Areas.Admin.Controllers;

public class QuoteRequestsController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuoteRequestsController> _logger;

    public QuoteRequestsController(ApplicationDbContext context, ILogger<QuoteRequestsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? recherche, QuoteStatus? statut)
    {
        ViewData["Title"] = "Demandes de devis";

        var query = _context.QuoteRequests.Include(q => q.Service).AsQueryable();

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            var term = recherche.Trim();
            query = query.Where(q =>
                q.Nom.Contains(term) ||
                q.Prenom.Contains(term) ||
                q.Email.Contains(term) ||
                q.NumeroDemande.Contains(term));
        }

        if (statut.HasValue)
            query = query.Where(q => q.Statut == statut.Value);

        ViewData["Recherche"] = recherche;
        ViewData["StatutSelectionne"] = statut;

        var demandes = await query.OrderByDescending(q => q.DateCreation).ToListAsync();
        return View(demandes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var demande = await _context.QuoteRequests
            .Include(q => q.Service)
            .Include(q => q.QuotePhotos)
            .Include(q => q.QuoteItems)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (demande is null)
            return NotFound();

        ViewData["Title"] = $"Demande {demande.NumeroDemande}";
        return View(demande);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangerStatut(int id, QuoteStatus statut)
    {
        var demande = await _context.QuoteRequests.FindAsync(id);
        if (demande is null)
            return NotFound();

        demande.Statut = statut;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Statut de la demande {Numero} changé en {Statut}.", demande.NumeroDemande, statut);

        TempData["StatusUpdated"] = "Statut mis à jour.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
