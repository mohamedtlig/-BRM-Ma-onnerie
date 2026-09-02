using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BRM.Web.ValidationAttributes;

/// <summary>
/// Force une case à cocher (bool non-nullable) à être cochée, côté serveur ET côté client.
/// Remplace le couple [Required]+[Range(typeof(bool),"true","true")], dont l'adaptateur de
/// validation jQuery compare "true"/"True" en chaînes et échoue toujours à cause de la casse.
/// </summary>
public class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
{
    public override bool IsValid(object? value) => value is bool b && b;

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-mustbetrue", ErrorMessage ?? "Ce champ doit être coché.");
    }
}
