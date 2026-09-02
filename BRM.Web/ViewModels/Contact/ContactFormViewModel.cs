using System.ComponentModel.DataAnnotations;
using BRM.Web.ValidationAttributes;

namespace BRM.Web.ViewModels.Contact;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Merci d'indiquer votre nom.")]
    [StringLength(150)]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer votre e-mail.")]
    [EmailAddress(ErrorMessage = "Adresse e-mail invalide.")]
    [StringLength(200)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    [StringLength(20)]
    [Display(Name = "Téléphone")]
    public string? Telephone { get; set; }

    [Required(ErrorMessage = "Merci de décrire votre demande.")]
    [StringLength(2000)]
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vous devez accepter l'utilisation de vos données pour être recontacté.")]
    [MustBeTrue(ErrorMessage = "Vous devez accepter l'utilisation de vos données pour être recontacté.")]
    [Display(Name = "J'accepte que mes informations soient utilisées pour traiter ma demande.")]
    public bool ConsentementRGPD { get; set; }
}
