using System.ComponentModel.DataAnnotations;

namespace BRM.Web.Models;

public class QuotePhoto
{
    public int Id { get; set; }

    public int QuoteRequestId { get; set; }
    public QuoteRequest? QuoteRequest { get; set; }

    [Required, StringLength(300)]
    public string CheminFichier { get; set; } = string.Empty;

    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
}
