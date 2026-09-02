using BRM.Web.Models;

namespace BRM.Web.ViewModels.Admin;

public class DashboardViewModel
{
    public int NouvellesDemandes { get; set; }
    public int DemandesATraiter { get; set; }
    public int VisitesPrevues { get; set; }
    public int DevisAcceptes { get; set; }

    public List<QuoteRequest> DernieresDemandes { get; set; } = new();
}
