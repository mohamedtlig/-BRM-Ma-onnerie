using BRM.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Controllers;

public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Nos services de maçonnerie — BRM";
        ViewData["MetaDescription"] = "Découvrez les prestations de maçonnerie générale de BRM à La Garde : rénovation, murs, dalles, façades, terrasses, aménagement extérieur.";

        var services = await _context.Services
            .Where(s => s.Actif)
            .OrderBy(s => s.Ordre)
            .ToListAsync();

        return View(services);
    }
}
