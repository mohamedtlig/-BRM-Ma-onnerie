using Microsoft.AspNetCore.Mvc;

namespace BRM.Web.Controllers;

public class LegalController : Controller
{
    public IActionResult MentionsLegales()
    {
        ViewData["Title"] = "Mentions légales — BRM";
        return View();
    }

    public IActionResult Confidentialite()
    {
        ViewData["Title"] = "Politique de confidentialité — BRM";
        return View();
    }
}
