using CloudStaff.DataContext;
using CloudStaff.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudStaff.MVC.Features.Admin;

public class AdminController : Controller
{
    private readonly CloudStaffContext _db;

    public AdminController(CloudStaffContext db)
    {
        _db = db;
    }

    // GET /Admin/Seed
    public IActionResult Seed()
    {
        return View();
    }

    // POST /Admin/Seed/Platforms
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedPlatforms(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Platforms_Error"] = "No file selected.";
            return RedirectToAction(nameof(Seed));
        }

        int created = 0, skipped = 0;
        var errors = new List<string>();
        var existing = await _db.Platforms.Select(p => p.Name.ToLower()).ToHashSetAsync();

        foreach (var (row, lineNum) in ReadCsvRows(file))
        {
            if (row.Length < 1 || string.IsNullOrWhiteSpace(row[0]))
            {
                errors.Add($"Line {lineNum}: empty name, skipped.");
                continue;
            }

            var name = row[0].Trim();
            if (existing.Contains(name.ToLower()))
            {
                skipped++;
                continue;
            }

            _db.Platforms.Add(new Platform { Name = name });
            existing.Add(name.ToLower());
            created++;
        }

        await _db.SaveChangesAsync();
        TempData["Platforms_Result"] = FormatResult("Platforms", created, skipped, errors);
        return RedirectToAction(nameof(Seed));
    }

    // POST /Admin/Seed/StaffCategories
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedStaffCategories(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Categories_Error"] = "No file selected.";
            return RedirectToAction(nameof(Seed));
        }

        int created = 0, skipped = 0;
        var errors = new List<string>();
        var existing = await _db.StaffCategories.Select(c => c.Name.ToLower()).ToHashSetAsync();

        foreach (var (row, lineNum) in ReadCsvRows(file))
        {
            if (row.Length < 1 || string.IsNullOrWhiteSpace(row[0]))
            {
                errors.Add($"Line {lineNum}: empty name, skipped.");
                continue;
            }

            var name = row[0].Trim();
            if (existing.Contains(name.ToLower()))
            {
                skipped++;
                continue;
            }

            _db.StaffCategories.Add(new StaffCategory { Name = name });
            existing.Add(name.ToLower());
            created++;
        }

        await _db.SaveChangesAsync();
        TempData["Categories_Result"] = FormatResult("Staff Categories", created, skipped, errors);
        return RedirectToAction(nameof(Seed));
    }

    // POST /Admin/Seed/HomePools
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedHomePools(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["HomePools_Error"] = "No file selected.";
            return RedirectToAction(nameof(Seed));
        }

        int created = 0, skipped = 0;
        var errors = new List<string>();

        var categories = await _db.StaffCategories.ToListAsync();
        var existingCodes = await _db.HomePools.Select(h => h.Code.ToLower()).ToHashSetAsync();

        foreach (var (row, lineNum) in ReadCsvRows(file))
        {
            if (row.Length < 3)
            {
                errors.Add($"Line {lineNum}: expected 3 columns (Code, Name, Category), got {row.Length}.");
                continue;
            }

            var code = row[0].Trim();
            var name = row[1].Trim();
            var categoryName = row[2].Trim();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            {
                errors.Add($"Line {lineNum}: Code and Name are required.");
                continue;
            }

            if (existingCodes.Contains(code.ToLower()))
            {
                skipped++;
                continue;
            }

            var category = categories.FirstOrDefault(c =>
                c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category is null)
            {
                errors.Add($"Line {lineNum}: Category '{categoryName}' not found — import Staff Categories first.");
                continue;
            }

            _db.HomePools.Add(new HomePool { Code = code, Name = name, CategoryId = category.Id });
            existingCodes.Add(code.ToLower());
            created++;
        }

        await _db.SaveChangesAsync();
        TempData["HomePools_Result"] = FormatResult("Home Pools", created, skipped, errors);
        return RedirectToAction(nameof(Seed));
    }

    // POST /Admin/Seed/StaffRoles
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedStaffRoles(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Roles_Error"] = "No file selected.";
            return RedirectToAction(nameof(Seed));
        }

        int created = 0, skipped = 0;
        var errors = new List<string>();

        var homePools = await _db.HomePools.ToListAsync();
        var existingRoles = await _db.StaffRoles
            .Select(r => r.Title.ToLower() + "|" + r.HomePoolId)
            .ToHashSetAsync();

        foreach (var (row, lineNum) in ReadCsvRows(file))
        {
            if (row.Length < 2)
            {
                errors.Add($"Line {lineNum}: expected 2 columns (Title, HomePoolCode), got {row.Length}.");
                continue;
            }

            var title = row[0].Trim();
            var homePoolCode = row[1].Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                errors.Add($"Line {lineNum}: Title is required.");
                continue;
            }

            var homePool = homePools.FirstOrDefault(h =>
                h.Code.Equals(homePoolCode, StringComparison.OrdinalIgnoreCase));

            if (homePool is null)
            {
                errors.Add($"Line {lineNum}: HomePool '{homePoolCode}' not found — import Home Pools first.");
                continue;
            }

            var key = title.ToLower() + "|" + homePool.Id;
            if (existingRoles.Contains(key))
            {
                skipped++;
                continue;
            }

            _db.StaffRoles.Add(new StaffRole { Title = title, HomePoolId = homePool.Id });
            existingRoles.Add(key);
            created++;
        }

        await _db.SaveChangesAsync();
        TempData["Roles_Result"] = FormatResult("Staff Roles", created, skipped, errors);
        return RedirectToAction(nameof(Seed));
    }

    // POST /Admin/Seed/Staff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedStaff(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Staff_Error"] = "No file selected.";
            return RedirectToAction(nameof(Seed));
        }

        // Materialise all rows so we can do two passes over the data
        var allRows = ReadCsvRows(file).ToList();
        var errors = new List<string>();

        // ── Pass 1: ensure every referenced manager exists ────────────────
        var managers = await _db.Managers.ToListAsync();
        var autoCreatedManagers = new List<string>();
        var seenManagerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (row, _) in allRows)
        {
            if (row.Length < 3) continue;
            var managerName = row[2].Trim();
            if (string.IsNullOrWhiteSpace(managerName) || seenManagerNames.Contains(managerName)) continue;
            seenManagerNames.Add(managerName);

            if (!managers.Any(m => m.Name.Equals(managerName, StringComparison.OrdinalIgnoreCase)))
            {
                var newManager = new Manager
                {
                    Name = managerName,
                    AsNumber = $"PENDING-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                    Location = string.Empty,
                    Email = string.Empty,
                };
                _db.Managers.Add(newManager);
                managers.Add(newManager);
                autoCreatedManagers.Add(managerName);
            }
        }

        if (autoCreatedManagers.Count > 0)
            await _db.SaveChangesAsync();

        // Reload so auto-created managers have their real IDs
        managers = await _db.Managers.ToListAsync();

        // ── Pass 2: create staff rows ──────────────────────────────────────
        var homePools = await _db.HomePools.ToListAsync();
        var staffRoles = await _db.StaffRoles.ToListAsync();
        var platforms = await _db.Platforms.ToListAsync();
        var existingAsNumbers = await _db.Staff.Select(s => s.AsNumber.ToLower()).ToHashSetAsync();
        int created = 0, skipped = 0;

        foreach (var (row, lineNum) in allRows)
        {
            // Name, AsNumber, ManagerName, HomePoolCode, RoleTitle, GcmLevel, SfiaLevel,
            // ContractJobTitle, PrimaryPlatform, SecondaryPlatform, TertiaryPlatform
            if (row.Length < 3)
            {
                errors.Add($"Line {lineNum}: expected at least 3 columns (Name, AsNumber, ManagerName).");
                continue;
            }

            var name = row[0].Trim();
            var asNumber = row[1].Trim();
            var managerName = row[2].Trim();
            var homePoolCode = row.Length > 3 ? row[3].Trim() : string.Empty;
            var roleTitle = row.Length > 4 ? row[4].Trim() : string.Empty;
            var gcmLevelStr = row.Length > 5 ? row[5].Trim() : string.Empty;
            var sfiaLevelStr = row.Length > 6 ? row[6].Trim() : string.Empty;
            var contractJobTitle = row.Length > 7 ? row[7].Trim() : null;
            var platformCols = new[]
            {
                row.Length > 8  ? row[8].Trim()  : string.Empty,
                row.Length > 9  ? row[9].Trim()  : string.Empty,
                row.Length > 10 ? row[10].Trim() : string.Empty,
            };

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(asNumber))
            {
                errors.Add($"Line {lineNum}: Name and AsNumber are required.");
                continue;
            }

            if (existingAsNumbers.Contains(asNumber.ToLower()))
            {
                skipped++;
                continue;
            }

            var manager = managers.FirstOrDefault(m =>
                m.Name.Equals(managerName, StringComparison.OrdinalIgnoreCase));

            if (manager is null)
            {
                // Shouldn't happen after pass 1, but guard anyway
                errors.Add($"Line {lineNum}: Manager '{managerName}' could not be resolved.");
                continue;
            }

            // If this staff member is themselves a manager that was auto-created with a
            // PENDING AsNumber, replace it now with their real AsNumber from the CSV.
            var selfAsManager = managers.FirstOrDefault(m =>
                m.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                m.AsNumber.StartsWith("PENDING-"));

            if (selfAsManager is not null)
                selfAsManager.AsNumber = asNumber;

            HomePool? homePool = null;
            if (!string.IsNullOrWhiteSpace(homePoolCode))
            {
                homePool = homePools.FirstOrDefault(h =>
                    h.Code.Equals(homePoolCode, StringComparison.OrdinalIgnoreCase));

                if (homePool is null)
                    errors.Add($"Line {lineNum}: HomePool '{homePoolCode}' not found — HomePool left blank for this row.");
            }

            StaffRole? role = null;
            if (!string.IsNullOrWhiteSpace(roleTitle) && homePool is not null)
            {
                role = staffRoles.FirstOrDefault(r =>
                    r.Title.Equals(roleTitle, StringComparison.OrdinalIgnoreCase) &&
                    r.HomePoolId == homePool.Id);

                if (role is null)
                    errors.Add($"Line {lineNum}: Role '{roleTitle}' not found in pool '{homePoolCode}' — Role left blank for this row.");
            }

            int? gcmLevel = null;
            if (!string.IsNullOrWhiteSpace(gcmLevelStr) && int.TryParse(gcmLevelStr, out var gcm) && gcm >= 1 && gcm <= 9)
                gcmLevel = gcm;

            int? sfiaLevel = null;
            if (!string.IsNullOrWhiteSpace(sfiaLevelStr) && int.TryParse(sfiaLevelStr, out var sfia) && sfia >= 1 && sfia <= 7)
                sfiaLevel = sfia;

            var staffEntity = new CloudStaff.EntityModels.Staff
            {
                Name = name,
                AsNumber = asNumber,
                ManagerId = manager.Id,
                HomePoolId = homePool?.Id,
                StaffRoleId = role?.Id,
                GcmLevel = gcmLevel,
                SfiaLevel = sfiaLevel,
                ContractJobTitle = string.IsNullOrWhiteSpace(contractJobTitle) ? null : contractJobTitle,
            };
            _db.Staff.Add(staffEntity);

            // Add platform associations via the navigation property — EF resolves StaffId on save
            var seenPlatformIds = new HashSet<int>();
            foreach (var platformName in platformCols)
            {
                if (string.IsNullOrWhiteSpace(platformName)) continue;
                var platform = platforms.FirstOrDefault(p =>
                    p.Name.Equals(platformName, StringComparison.OrdinalIgnoreCase));
                if (platform is null)
                {
                    errors.Add($"Line {lineNum}: Platform '{platformName}' not found — skipped.");
                    continue;
                }
                if (seenPlatformIds.Add(platform.Id))
                    staffEntity.StaffPlatforms.Add(new StaffPlatform { PlatformId = platform.Id });
            }

            existingAsNumbers.Add(asNumber.ToLower());
            created++;
        }

        // Final save picks up any remaining manager AsNumber updates and StaffPlatform rows
        await _db.SaveChangesAsync();

        // Build summary
        var sb = new System.Text.StringBuilder(FormatResult("Staff", created, skipped, errors));

        if (autoCreatedManagers.Count > 0)
        {
            sb.Append($" | ⚠ {autoCreatedManagers.Count} manager(s) auto-created from staff data");
            sb.Append(" — Location & Email are blank, please update via the Managers page: ");
            sb.Append(string.Join(", ", autoCreatedManagers));
        }

        TempData["Staff_Result"] = sb.ToString();
        return RedirectToAction(nameof(Seed));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Reads CSV rows from an uploaded file, skipping the header row.
    /// Returns (fields, 1-based line number). Does not handle quoted commas.
    /// </summary>
    private static IEnumerable<(string[] Row, int LineNum)> ReadCsvRows(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        int lineNum = 0;
        bool headerSkipped = false;

        while (!reader.EndOfStream)
        {
            lineNum++;
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (!headerSkipped) { headerSkipped = true; continue; }

            yield return (line.Split(',').Select(f => f.Trim()).ToArray(), lineNum);
        }
    }

    private static string FormatResult(string label, int created, int skipped, List<string> errors)
    {
        var parts = new List<string> { $"{created} created", $"{skipped} skipped" };
        if (errors.Count > 0)
            parts.Add($"{errors.Count} error(s): " + string.Join(" | ", errors));
        return $"{label}: " + string.Join(", ", parts);
    }
}
