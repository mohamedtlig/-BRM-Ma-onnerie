using BRM.Web.Data;
using BRM.Web.Interfaces;
using BRM.Web.Models;
using BRM.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Areas.Admin.Controllers;

public class ProjectsController : AdminBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IFileUploadService _fileUploadService;

    public ProjectsController(ApplicationDbContext context, IFileUploadService fileUploadService)
    {
        _context = context;
        _fileUploadService = fileUploadService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Gestion des réalisations";
        var projects = await _context.Projects.OrderByDescending(p => p.Date).ToListAsync();
        return View(projects);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nouvelle réalisation";
        return View(new ProjectEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var project = new Project
        {
            Titre = model.Titre.Trim(),
            Description = model.Description.Trim(),
            TypeTravaux = model.TypeTravaux.Trim(),
            Ville = model.Ville.Trim(),
            Date = model.Date,
            EstAvantApres = model.EstAvantApres,
            Publie = model.Publie
        };

        if (model.PhotoPrincipale is not null)
        {
            try
            {
                project.PhotoPrincipale = await _fileUploadService.EnregistrerImageAsync(model.PhotoPrincipale, "projects");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.PhotoPrincipale), ex.Message);
                return View(model);
            }
        }

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        if (model.PhotosSupplementaires is not null)
        {
            var ordre = 0;
            foreach (var photo in model.PhotosSupplementaires.Where(p => p.Length > 0).Take(20))
            {
                try
                {
                    var path = await _fileUploadService.EnregistrerImageAsync(photo, "projects");
                    project.ProjectPhotos.Add(new ProjectPhoto { CheminFichier = path, Ordre = ordre++ });
                }
                catch (InvalidOperationException)
                {
                    // Photo ignorée si elle ne respecte pas les contraintes ; les autres continuent d'être traitées.
                }
            }
            await _context.SaveChangesAsync();
        }

        TempData["ProjectSaved"] = "Réalisation créée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _context.Projects.Include(p => p.ProjectPhotos).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
            return NotFound();

        ViewData["Title"] = $"Modifier — {project.Titre}";
        ViewData["PhotosExistantes"] = project.ProjectPhotos.OrderBy(p => p.Ordre).ToList();

        var model = new ProjectEditViewModel
        {
            Id = project.Id,
            Titre = project.Titre,
            Description = project.Description,
            TypeTravaux = project.TypeTravaux,
            Ville = project.Ville,
            Date = project.Date,
            PhotoPrincipaleExistante = project.PhotoPrincipale,
            EstAvantApres = project.EstAvantApres,
            Publie = project.Publie
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectEditViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        var project = await _context.Projects.Include(p => p.ProjectPhotos).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["PhotosExistantes"] = project.ProjectPhotos.OrderBy(p => p.Ordre).ToList();
            return View(model);
        }

        project.Titre = model.Titre.Trim();
        project.Description = model.Description.Trim();
        project.TypeTravaux = model.TypeTravaux.Trim();
        project.Ville = model.Ville.Trim();
        project.Date = model.Date;
        project.EstAvantApres = model.EstAvantApres;
        project.Publie = model.Publie;

        if (model.PhotoPrincipale is not null)
        {
            try
            {
                project.PhotoPrincipale = await _fileUploadService.EnregistrerImageAsync(model.PhotoPrincipale, "projects");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.PhotoPrincipale), ex.Message);
                ViewData["PhotosExistantes"] = project.ProjectPhotos.OrderBy(p => p.Ordre).ToList();
                return View(model);
            }
        }

        if (model.PhotosSupplementaires is not null)
        {
            var ordre = project.ProjectPhotos.Count;
            foreach (var photo in model.PhotosSupplementaires.Where(p => p.Length > 0).Take(20))
            {
                try
                {
                    var path = await _fileUploadService.EnregistrerImageAsync(photo, "projects");
                    project.ProjectPhotos.Add(new ProjectPhoto { CheminFichier = path, Ordre = ordre++ });
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        await _context.SaveChangesAsync();

        TempData["ProjectSaved"] = "Réalisation mise à jour avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int id, int projectId)
    {
        var photo = await _context.ProjectPhotos.FindAsync(id);
        if (photo is not null)
        {
            _context.ProjectPhotos.Remove(photo);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Edit), new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null)
            return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        TempData["ProjectSaved"] = "Réalisation supprimée.";
        return RedirectToAction(nameof(Index));
    }
}
