using System.Diagnostics;
using BRM.Web.Data;
using BRM.Web.Models;
using BRM.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Entreprise de maçonnerie générale à La Garde, Toulon (Var)";
        ViewData["MetaDescription"] = "BRM, entreprise de maçonnerie générale basée à La Garde, intervient à Toulon et dans le Var : rénovation, murs, dalles, façades, terrasses. Demandez votre estimation en ligne.";

        var model = new HomeIndexViewModel
        {
            ServicesPhares = await _context.Services
                .Where(s => s.Actif)
                .OrderBy(s => s.Ordre)
                .Take(6)
                .ToListAsync(),
            RealisationsRecentes = await _context.Projects
                .Where(p => p.Publie)
                .OrderByDescending(p => p.Date)
                .Take(3)
                .ToListAsync()
        };

        return View(model);
    }

    public IActionResult About()
    {
        ViewData["Title"] = "À propos de BRM — Maçonnerie générale";
        ViewData["MetaDescription"] = "Découvrez BRM, entreprise de maçonnerie générale à La Garde (Var), et son intervention sur Toulon et son agglomération.";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCodePage(int id)
    {
        ViewData["Title"] = id == 404 ? "Page introuvable" : "Erreur";
        ViewData["StatusCode"] = id;
        Response.StatusCode = id;
        return View();
    }
}
