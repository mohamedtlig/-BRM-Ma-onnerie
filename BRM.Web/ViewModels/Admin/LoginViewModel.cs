using System.ComponentModel.DataAnnotations;

namespace BRM.Web.ViewModels.Admin;

public class LoginViewModel
{
    [Required(ErrorMessage = "Merci d'indiquer votre e-mail.")]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Merci d'indiquer votre mot de passe.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Se souvenir de moi")]
    public bool RememberMe { get; set; }
}
