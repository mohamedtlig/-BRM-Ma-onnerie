using BRM.Web.Data;
using BRM.Web.Models.Enums;
using BRM.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Areas.Admin.Controllers;

public class DashboardController : AdminBaseController
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Tableau de bord";

        var model = new DashboardViewModel
        {
            NouvellesDemandes = await _context.QuoteRequests.CountAsync(q => q.Statut == QuoteStatus.Nouvelle),
            DemandesATraiter = await _context.QuoteRequests.CountAsync(q =>
                q.Statut == QuoteStatus.Nouvelle || q.Statut == QuoteStatus.EnEtude || q.Statut == QuoteStatus.ClientContacte),
            VisitesPrevues = await _context.QuoteRequests.CountAsync(q => q.Statut == QuoteStatus.VisitePlanifiee),
            DevisAcceptes = await _context.QuoteRequests.CountAsync(q => q.Statut == QuoteStatus.Accepte),
            DernieresDemandes = await _context.QuoteRequests
                .Include(q => q.Service)
                .OrderByDescending(q => q.DateCreation)
                .Take(8)
                .ToListAsync()
        };

        return View(model);
    }
}
