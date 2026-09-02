using BRM.Web.Data;
using BRM.Web.Interfaces;
using BRM.Web.Models;
using BRM.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Areas.Admin.Controllers;

public class ServicesController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IFileUploadService _fileUploadService;

    public ServicesController(ApplicationDbContext context, IFileUploadService fileUploadService)
    {
        _context = context;
        _fileUploadService = fileUploadService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Gestion des services";
        var services = await _context.Services
            .Include(s => s.ServicePricing)
            .OrderBy(s => s.Ordre)
            .ToListAsync();
        return View(services);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nouveau service";
        return View(new ServiceEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var service = new Service
        {
            Nom = model.Nom.Trim(),
            Description = model.Description.Trim(),
            Unite = model.Unite.Trim(),
            Actif = model.Actif,
            Ordre = model.Ordre,
            ServicePricing = new ServicePricing
            {
                PrixBase = model.PrixBase,
                PrixParUnite = model.PrixParUnite,
                CoefficientComplexite = model.CoefficientComplexite,
                FraisFixes = model.FraisFixes,
                MargeFourchette = model.MargeFourchette,
                DateMiseAJour = DateTime.UtcNow
            }
        };

        if (model.Image is not null)
        {
            try
            {
                service.ImagePath = await _fileUploadService.EnregistrerImageAsync(model.Image, "services");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.Image), ex.Message);
                return View(model);
            }
        }

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        TempData["ServiceSaved"] = "Service créé avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await _context.Services.Include(s => s.ServicePricing).FirstOrDefaultAsync(s => s.Id == id);
        if (service is null)
            return NotFound();

        ViewData["Title"] = $"Modifier — {service.Nom}";

        var model = new ServiceEditViewModel
        {
            Id = service.Id,
            Nom = service.Nom,
            Description = service.Description,
            Unite = service.Unite,
            Actif = service.Actif,
            Ordre = service.Ordre,
            ImagePath = service.ImagePath,
            PrixBase = service.ServicePricing?.PrixBase ?? 0,
            PrixParUnite = service.ServicePricing?.PrixParUnite ?? 0,
            CoefficientComplexite = service.ServicePricing?.CoefficientComplexite ?? 1.0m,
            FraisFixes = service.ServicePricing?.FraisFixes ?? 0,
            MargeFourchette = service.ServicePricing?.MargeFourchette ?? 0.15m
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceEditViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        var service = await _context.Services.Include(s => s.ServicePricing).FirstOrDefaultAsync(s => s.Id == id);
        if (service is null)
            return NotFound();

        service.Nom = model.Nom.Trim();
        service.Description = model.Description.Trim();
        service.Unite = model.Unite.Trim();
        service.Actif = model.Actif;
        service.Ordre = model.Ordre;

        if (model.Image is not null)
        {
            try
            {
                service.ImagePath = await _fileUploadService.EnregistrerImageAsync(model.Image, "services");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.Image), ex.Message);
                return View(model);
            }
        }

        service.ServicePricing ??= new ServicePricing { ServiceId = service.Id };
        service.ServicePricing.PrixBase = model.PrixBase;
        service.ServicePricing.PrixParUnite = model.PrixParUnite;
        service.ServicePricing.CoefficientComplexite = model.CoefficientComplexite;
        service.ServicePricing.FraisFixes = model.FraisFixes;
        service.ServicePricing.MargeFourchette = model.MargeFourchette;
        service.ServicePricing.DateMiseAJour = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["ServiceSaved"] = "Service mis à jour avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service is null)
            return NotFound();

        var utilise = await _context.QuoteRequests.AnyAsync(q => q.ServiceId == id);
        if (utilise)
        {
            TempData["ServiceError"] = "Impossible de supprimer ce service : des demandes de devis y sont rattachées. Désactivez-le plutôt.";
            return RedirectToAction(nameof(Index));
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        TempData["ServiceSaved"] = "Service supprimé.";
        return RedirectToAction(nameof(Index));
    }
}
