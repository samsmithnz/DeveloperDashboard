using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DeveloperDashboard.Web.Models;

namespace DeveloperDashboard.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index(string? search, string? team)
    {
        var projects = PrototypeData.Projects.Where(r =>
            (string.IsNullOrWhiteSpace(search) || r.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || r.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(team) || r.Team == team)).ToArray();
        return View(new DashboardViewModel(projects, search, team));
    }

    public IActionResult Project(string id)
    {
        var project = PrototypeData.Projects.SingleOrDefault(p => p.Slug == id);
        return project is null ? NotFound() : View(project);
    }

    public IActionResult Repository(string id)
    {
        var repository = PrototypeData.Repositories.SingleOrDefault(r => r.Slug == id);
        return repository is null ? NotFound() : View(repository);
    }

    public IActionResult Recommendations(string? severity)
    {
        ViewData["Severity"] = severity;
        return View(PrototypeData.Recommendations
            .Where(r => string.IsNullOrEmpty(severity) || r.Severity == severity).ToArray());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
