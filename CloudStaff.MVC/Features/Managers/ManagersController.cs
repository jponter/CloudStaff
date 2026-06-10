using CloudStaff.DataContext;
using CloudStaff.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudStaff.MVC.Features.Managers;

public class ManagersController : Controller
{
    private readonly CloudStaffContext _db;

    public ManagersController(CloudStaffContext db)
    {
        _db = db;
    }

    // GET /Managers
    public async Task<IActionResult> Index()
    {
        var managers = await _db.Managers
            .Select(m => new ManagerViewModel
            {
                Id = m.Id,
                Name = m.Name,
                AsNumber = m.AsNumber,
                Location = m.Location,
                Email = m.Email,
                StaffCount = m.Staff.Count()
            })
            .OrderBy(m => m.Name)
            .ToListAsync();

        return View(managers);
    }

    // GET /Managers/Create
    public IActionResult Create()
    {
        return View(new ManagerViewModel());
    }

    // POST /Managers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ManagerViewModel vm)
    {
        if (await _db.Managers.AnyAsync(m => m.AsNumber == vm.AsNumber))
            ModelState.AddModelError(nameof(vm.AsNumber), "AS Number is already in use.");

        if (!ModelState.IsValid)
            return View(vm);

        _db.Managers.Add(new Manager
        {
            Name = vm.Name,
            AsNumber = vm.AsNumber,
            Location = vm.Location,
            Email = vm.Email
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Managers/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var manager = await _db.Managers.FindAsync(id);
        if (manager is null) return NotFound();

        return View(new ManagerViewModel
        {
            Id = manager.Id,
            Name = manager.Name,
            AsNumber = manager.AsNumber,
            Location = manager.Location,
            Email = manager.Email
        });
    }

    // POST /Managers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ManagerViewModel vm)
    {
        if (await _db.Managers.AnyAsync(m => m.AsNumber == vm.AsNumber && m.Id != id))
            ModelState.AddModelError(nameof(vm.AsNumber), "AS Number is already in use.");

        if (!ModelState.IsValid)
            return View(vm);

        var manager = await _db.Managers.FindAsync(id);
        if (manager is null) return NotFound();

        manager.Name = vm.Name;
        manager.AsNumber = vm.AsNumber;
        manager.Location = vm.Location;
        manager.Email = vm.Email;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET /Managers/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var manager = await _db.Managers
            .Include(m => m.Staff)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (manager is null) return NotFound();

        var vm = new ManagerViewModel
        {
            Id = manager.Id,
            Name = manager.Name,
            AsNumber = manager.AsNumber,
            Location = manager.Location,
            Email = manager.Email,
            StaffCount = manager.Staff.Count
        };

        return View(vm);
    }

    // POST /Managers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var manager = await _db.Managers
            .Include(m => m.Staff)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (manager is null) return NotFound();

        if (manager.Staff.Count > 0)
        {
            ModelState.AddModelError(string.Empty,
                "This manager has staff assigned. Reassign them before deleting.");

            return View(new ManagerViewModel
            {
                Id = manager.Id,
                Name = manager.Name,
                AsNumber = manager.AsNumber,
                Location = manager.Location,
                Email = manager.Email,
                StaffCount = manager.Staff.Count
            });
        }

        _db.Managers.Remove(manager);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
