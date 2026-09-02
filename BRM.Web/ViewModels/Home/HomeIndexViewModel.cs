using BRM.Web.Models;

namespace BRM.Web.ViewModels.Home;

public class HomeIndexViewModel
{
    public List<Service> ServicesPhares { get; set; } = new();
    public List<Project> RealisationsRecentes { get; set; } = new();
}
