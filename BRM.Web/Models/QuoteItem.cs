using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BRM.Web.Models;

/// <summary>Ligne supplémentaire pouvant détailler une demande de devis (option, prestation additionnelle).</summary>
public class QuoteItem
{
    public int Id { get; set; }

    public int QuoteRequestId { get; set; }
    public QuoteRequest? QuoteRequest { get; set; }

    [Required, StringLength(200)]
    public string Libelle { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal MontantEstime { get; set; }
}
