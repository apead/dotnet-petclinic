using PetClinic.Data;
using PetClinic.Models;

namespace PetClinic.Services;

public class VisitService(PetClinicContext db)
{
    public async Task CreateAsync(Visit visit)
    {
        db.Visits.Add(visit);
        await db.SaveChangesAsync();
    }
}
