using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PetClinic.Data;
using PetClinic.Models;

namespace PetClinic.Services;

public class VetService(PetClinicContext db, IMemoryCache cache)
{
    private const string CacheKey = "vets";

    public async Task<List<Vet>> GetAllAsync()
    {
        if (!cache.TryGetValue(CacheKey, out List<Vet>? vets))
        {
            vets = await db.Vets.Include(v => v.Specialties).OrderBy(v => v.LastName).ToListAsync();
            cache.Set(CacheKey, vets, TimeSpan.FromMinutes(10));
        }
        return vets!;
    }

    public async Task<(List<Vet> Items, int TotalCount)> GetPagedAsync(int page, int pageSize = 5)
    {
        var all = await GetAllAsync();
        return (all.Skip((page - 1) * pageSize).Take(pageSize).ToList(), all.Count);
    }
}
