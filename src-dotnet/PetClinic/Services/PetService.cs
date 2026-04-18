using Microsoft.EntityFrameworkCore;
using PetClinic.Data;
using PetClinic.Models;

namespace PetClinic.Services;

public class PetService(PetClinicContext db)
{
    public async Task<List<PetType>> GetPetTypesAsync() =>
        await db.PetTypes.OrderBy(t => t.Name).ToListAsync();

    public async Task<Pet?> GetByIdAsync(int id) =>
        await db.Pets.Include(p => p.Type).Include(p => p.Visits).ThenInclude(v => v.Vet).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> IsDuplicateNameAsync(int ownerId, string name, int? excludePetId = null) =>
        await db.Pets.AnyAsync(p =>
            p.OwnerId == ownerId &&
            p.Name.ToLower() == name.ToLower() &&
            (excludePetId == null || p.Id != excludePetId));

    public async Task CreateAsync(Pet pet, int ownerId)
    {
        pet.OwnerId = ownerId;
        db.Pets.Add(pet);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pet pet)
    {
        db.Pets.Update(pet);
        await db.SaveChangesAsync();
    }
}
