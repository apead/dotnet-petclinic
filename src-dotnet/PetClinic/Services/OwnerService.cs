using Microsoft.EntityFrameworkCore;
using PetClinic.Data;
using PetClinic.Models;

namespace PetClinic.Services;

public class OwnerService(PetClinicContext db)
{
    public async Task<(List<Owner> Items, int TotalCount)> SearchAsync(string lastName, int page, int pageSize = 5)
    {
        var q = db.Owners
            .Include(o => o.Pets).ThenInclude(p => p.Type)
            .Include(o => o.Pets).ThenInclude(p => p.Visits)
            .Where(o => o.LastName.StartsWith(lastName))
            .OrderBy(o => o.LastName);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Owner?> GetByIdAsync(int id) =>
        await db.Owners
            .Include(o => o.Pets).ThenInclude(p => p.Type)
            .Include(o => o.Pets).ThenInclude(p => p.Visits)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Owner> CreateAsync(Owner owner)
    {
        db.Owners.Add(owner);
        await db.SaveChangesAsync();
        return owner;
    }

    public async Task UpdateAsync(Owner owner)
    {
        db.Owners.Update(owner);
        await db.SaveChangesAsync();
    }
}
