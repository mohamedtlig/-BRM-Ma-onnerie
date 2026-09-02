using BRM.Web.Data;
using BRM.Web.ViewModels.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? type)
    {
        ViewData["Title"] = "Nos réalisations — BRM Maçonnerie";
        ViewData["MetaDescription"] = "Découvrez les réalisations de BRM, entreprise de maçonnerie générale à La Garde, Toulon et dans le Var.";

        var query = _context.Projects.Where(p => p.Publie);

        var types = await query.Select(p => p.TypeTravaux).Distinct().OrderBy(t => t).ToListAsync();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.TypeTravaux == type);

        var projects = await query
            .Include(p => p.ProjectPhotos)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        return View(new ProjectListViewModel
        {
            Realisations = projects,
            TypesDisponibles = types,
            TypeSelectionne = type
        });
    }
}
