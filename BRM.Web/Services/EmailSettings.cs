namespace BRM.Web.Services;

/// <summary>
/// Lié à la section "EmailSettings" de la configuration. En développement, les valeurs sensibles
/// (SmtpUser, SmtpPassword) doivent être placées dans les User Secrets, jamais commitées dans appsettings.json.
/// </summary>
public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "BRM";
    public string ManagerEmail { get; set; } = string.Empty;
}
