using System.ComponentModel.DataAnnotations;

namespace BRM.Web.Models;

public class ContactMessage
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Nom { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? Telephone { get; set; }

    [Required, StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    public DateTime DateEnvoi { get; set; } = DateTime.UtcNow;

    public bool Traite { get; set; }
}
