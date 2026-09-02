namespace BRM.Web.ViewModels.Quote;

public class QuoteResultViewModel
{
    public int QuoteRequestId { get; set; }
    public string NumeroDemande { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }

    public string Prenom { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string AdresseChantier { get; set; } = string.Empty;
    public string CodePostal { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;

    public string TypeTravaux { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Surface { get; set; }
    public decimal? Quantite { get; set; }

    public decimal EstimationBasse { get; set; }
    public decimal EstimationHaute { get; set; }

    public bool EmailEnvoye { get; set; }
}
