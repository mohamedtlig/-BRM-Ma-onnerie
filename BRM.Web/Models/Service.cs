using System.ComponentModel.DataAnnotations;

namespace BRM.Web.Models;

public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom du service est obligatoire.")]
    [StringLength(150)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est obligatoire.")]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(300)]
    public string? ImagePath { get; set; }

    [Required(ErrorMessage = "L'unité de facturation est obligatoire.")]
    [StringLength(50)]
    public string Unite { get; set; } = "m²";

    public bool Actif { get; set; } = true;

    public int Ordre { get; set; }

    public ServicePricing? ServicePricing { get; set; }

    public ICollection<QuoteRequest> QuoteRequests { get; set; } = new List<QuoteRequest>();
}
