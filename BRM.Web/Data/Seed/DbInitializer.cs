using BRM.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BRM.Web.Data.Seed;

/// <summary>
/// Applique les migrations en attente puis initialise les données minimales nécessaires
/// au fonctionnement du site (rôle Admin, compte gérant, catalogue de services de démonstration).
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        await context.Database.MigrateAsync();

        await SeedRolesAndAdminAsync(services, logger);
        await SeedServicesAsync(context);
        await SeedDemoProjectsAsync(context);
    }

    private static async Task SeedRolesAndAdminAsync(IServiceProvider services, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        var adminEmail = configuration["AdminAccount:Email"];
        var adminPassword = configuration["AdminAccount:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning(
                "Aucun compte gérant créé : configurez AdminAccount:Email et AdminAccount:Password " +
                "(via 'dotnet user-secrets' en développement) pour générer le premier compte administrateur.");
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            NomComplet = "Gérant BRM"
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, adminRole);
            logger.LogInformation("Compte administrateur initial créé pour {Email}.", adminEmail);
        }
        else
        {
            logger.LogError("Échec de création du compte administrateur : {Erreurs}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedServicesAsync(ApplicationDbContext context)
    {
        if (await context.Services.AnyAsync())
            return;

        var services = new List<Service>
        {
            new()
            {
                Nom = "Maçonnerie générale",
                Description = "Travaux de maçonnerie tous corps d'état : élévation de murs, fondations, structures en parpaing ou béton.",
                Unite = "m²",
                Ordre = 1,
                ServicePricing = new ServicePricing { PrixBase = 500, PrixParUnite = 120, CoefficientComplexite = 1.0m, FraisFixes = 0, MargeFourchette = 0.15m }
            },
            new()
            {
                Nom = "Rénovation",
                Description = "Rénovation partielle ou complète de bâtiments existants.",
                Unite = "m²",
                Ordre = 2,
                ServicePricing = new ServicePricing { PrixBase = 400, PrixParUnite = 150, CoefficientComplexite = 1.1m, FraisFixes = 0, MargeFourchette = 0.2m }
            },
            new()
            {
                Nom = "Murs et cloisons",
                Description = "Construction de murs porteurs, murs de séparation et cloisons intérieures.",
                Unite = "m²",
                Ordre = 3,
                ServicePricing = new ServicePricing { PrixBase = 200, PrixParUnite = 90, CoefficientComplexite = 1.0m, FraisFixes = 0, MargeFourchette = 0.15m }
            },
            new()
            {
                Nom = "Dalles et chapes",
                Description = "Coulage de dalles béton et chapes de ragréage.",
                Unite = "m²",
                Ordre = 4,
                ServicePricing = new ServicePricing { PrixBase = 300, PrixParUnite = 70, CoefficientComplexite = 1.0m, FraisFixes = 0, MargeFourchette = 0.15m }
            },
            new()
            {
                Nom = "Travaux de façade",
                Description = "Ravalement, enduit et réfection de façades.",
                Unite = "m²",
                Ordre = 5,
                ServicePricing = new ServicePricing { PrixBase = 350, PrixParUnite = 80, CoefficientComplexite = 1.05m, FraisFixes = 0, MargeFourchette = 0.2m }
            },
            new()
            {
                Nom = "Terrasse",
                Description = "Création de terrasses maçonnées, dallées ou en béton désactivé.",
                Unite = "m²",
                Ordre = 6,
                ServicePricing = new ServicePricing { PrixBase = 450, PrixParUnite = 110, CoefficientComplexite = 1.0m, FraisFixes = 0, MargeFourchette = 0.15m }
            },
            new()
            {
                Nom = "Démolition légère",
                Description = "Démolition partielle de murs, cloisons ou structures non porteuses.",
                Unite = "m²",
                Ordre = 7,
                ServicePricing = new ServicePricing { PrixBase = 250, PrixParUnite = 50, CoefficientComplexite = 1.0m, FraisFixes = 100, MargeFourchette = 0.2m }
            },
            new()
            {
                Nom = "Aménagement extérieur",
                Description = "Murets, allées, escaliers extérieurs et aménagements maçonnés du jardin.",
                Unite = "m²",
                Ordre = 8,
                ServicePricing = new ServicePricing { PrixBase = 300, PrixParUnite = 95, CoefficientComplexite = 1.0m, FraisFixes = 0, MargeFourchette = 0.2m }
            },
            new()
            {
                Nom = "Autres travaux de maçonnerie",
                Description = "Projet spécifique non listé ci-dessus : décrivez-le, BRM étudiera votre demande.",
                Unite = "forfait",
                Ordre = 9,
                ServicePricing = new ServicePricing { PrixBase = 300, PrixParUnite = 0, CoefficientComplexite = 1.0m, FraisFixes = 0, MargeFourchette = 0.3m }
            }
        };

        context.Services.AddRange(services);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Données d'exemple, sans photo (aucun visuel de chantier n'est inventé) : elles montrent le
    /// fonctionnement de la galerie et doivent être remplacées ou complétées par le gérant depuis l'admin.
    /// </summary>
    private static async Task SeedDemoProjectsAsync(ApplicationDbContext context)
    {
        if (await context.Projects.AnyAsync())
            return;

        context.Projects.AddRange(
            new Project
            {
                Titre = "[Exemple] Muret de clôture en parpaing",
                Description = "Exemple de fiche réalisation à compléter par le gérant : description du chantier, contraintes, résultat.",
                TypeTravaux = "Murs et cloisons",
                Ville = "La Garde",
                Date = DateTime.UtcNow.AddMonths(-2),
                Publie = false
            },
            new Project
            {
                Titre = "[Exemple] Terrasse en béton désactivé",
                Description = "Exemple de fiche réalisation à compléter par le gérant, avec photos avant/après si disponibles.",
                TypeTravaux = "Terrasse",
                Ville = "Toulon",
                Date = DateTime.UtcNow.AddMonths(-1),
                EstAvantApres = true,
                Publie = false
            }
        );

        await context.SaveChangesAsync();
    }
}
