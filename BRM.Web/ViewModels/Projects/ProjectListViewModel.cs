using BRM.Web.Models;

namespace BRM.Web.ViewModels.Projects;

public class ProjectListViewModel
{
    public List<Project> Realisations { get; set; } = new();
    public List<string> TypesDisponibles { get; set; } = new();
    public string? TypeSelectionne { get; set; }
}
