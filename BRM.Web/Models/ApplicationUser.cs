using Microsoft.AspNetCore.Identity;

namespace BRM.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string? NomComplet { get; set; }
}
