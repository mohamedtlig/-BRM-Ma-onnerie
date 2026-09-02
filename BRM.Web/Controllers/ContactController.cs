using BRM.Web.Data;
using BRM.Web.Models;
using BRM.Web.ViewModels.Contact;
using Microsoft.AspNetCore.Mvc;

namespace BRM.Web.Controllers;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContactController> _logger;

    public ContactController(ApplicationDbContext context, ILogger<ContactController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Contact — BRM Maçonnerie générale";
        ViewData["MetaDescription"] = "Contactez BRM, entreprise de maçonnerie générale à La Garde, pour toute question sur vos travaux à Toulon ou dans le Var.";
        return View(new ContactFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Contact — BRM Maçonnerie générale";
            return View(model);
        }

        var message = new ContactMessage
        {
            Nom = model.Nom,
            Email = model.Email,
            Telephone = model.Telephone,
            Message = model.Message
        };

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Nouveau message de contact reçu de {Email}.", model.Email);

        TempData["ContactSuccess"] = "Votre message a bien été envoyé. Nous reviendrons vers vous rapidement.";
        return RedirectToAction(nameof(Index));
    }
}
