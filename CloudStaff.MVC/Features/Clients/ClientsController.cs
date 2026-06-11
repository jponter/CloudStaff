using CloudStaff.DataContext;
using CloudStaff.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudStaff.MVC.Features.Clients;

public class ClientsController : Controller
{
    private readonly CloudStaffContext _db;

    public ClientsController(CloudStaffContext db)
    {
        _db = db;
    }

    // GET /Clients
    public async Task<IActionResult> Index()
    {
        var clients = await _db.Clients
            .Select(c => new ClientViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                ProjectCount = c.Projects.Count()
            })
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(clients);
    }

    // GET /Clients/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        var vm = new ClientViewModel
        {
            Id = client.Id,
            Name = client.Name,
            Description = client.Description,
            Type = client.Type,
            ProjectCount = await _db.ClientProjects.CountAsync(p => p.ClientId == id)
        };

        return View(vm);
    }

    // GET /Clients/ProjectsJson/5  — data source for AG Grid
    public async Task<IActionResult> ProjectsJson(int id)
    {
        var projects = await _db.ClientProjects
            .Where(p => p.ClientId == id)
            .OrderBy(p => p.Name)
            .Select(p => new { id = p.Id, clientId = p.ClientId, name = p.Name, description = p.Description })
            .ToListAsync();

        return Json(projects);
    }

    // POST /Clients/AddProject  — called by AG Grid toolbar
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> AddProject([FromBody] ClientProjectViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Name))
            return BadRequest("Name is required.");

        var project = new ClientProject
        {
            ClientId = vm.ClientId,
            Name = vm.Name.Trim(),
            Description = vm.Description?.Trim() ?? string.Empty,
        };
        _db.ClientProjects.Add(project);
        await _db.SaveChangesAsync();

        return Json(new { id = project.Id, clientId = project.ClientId, name = project.Name, description = project.Description });
    }

    // POST /Clients/UpdateProject  — called by AG Grid on cell edit
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdateProject([FromBody] ClientProjectViewModel vm)
    {
        var project = await _db.ClientProjects.FindAsync(vm.Id);
        if (project is null) return NotFound();

        project.Name = string.IsNullOrWhiteSpace(vm.Name) ? project.Name : vm.Name.Trim();
        project.Description = vm.Description?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }

    // POST /Clients/DeleteProject/5  — called by AG Grid row delete button
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _db.ClientProjects.FindAsync(id);
        if (project is null) return NotFound();

        _db.ClientProjects.Remove(project);
        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }

    // GET /Clients/Create
    public IActionResult Create()
    {
        return View(new ClientViewModel());
    }

    // POST /Clients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        _db.Clients.Add(new Client
        {
            Name = vm.Name,
            Description = vm.Description ?? string.Empty,
            Type = vm.Type,
        });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Clients/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        return View(new ClientViewModel
        {
            Id = client.Id,
            Name = client.Name,
            Description = client.Description,
            Type = client.Type,
        });
    }

    // POST /Clients/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClientViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var client = await _db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        client.Name = vm.Name;
        client.Description = vm.Description ?? string.Empty;
        client.Type = vm.Type;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET /Clients/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client is null) return NotFound();

        return View(new ClientViewModel
        {
            Id = client.Id,
            Name = client.Name,
            Description = client.Description,
            Type = client.Type,
            ProjectCount = client.Projects.Count
        });
    }

    // POST /Clients/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return NotFound();

        _db.Clients.Remove(client); // projects cascade
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
