using Microsoft.EntityFrameworkCore;
using PetClinic.Data;
using PetClinic.Models;
using PetClinic.Services;

namespace PetClinic.Tests.Services;

public class VisitServiceTests : IDisposable
{
    private readonly PetClinicContext _db;
    private readonly VisitService _sut;

    public VisitServiceTests()
    {
        var options = new DbContextOptionsBuilder<PetClinicContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new PetClinicContext(options);
        _sut = new VisitService(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_AddsVisitToDatabase()
    {
        var visit = new Visit
        {
            Description = "Annual checkup",
            PetId = 1
        };

        await _sut.CreateAsync(visit);

        Assert.Equal(1, await _db.Visits.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_AssignsIdToVisit()
    {
        var visit = new Visit
        {
            Description = "Vaccination",
            PetId = 2
        };

        await _sut.CreateAsync(visit);

        Assert.NotEqual(0, visit.Id);
    }

    [Fact]
    public async Task CreateAsync_PreservesVisitProperties()
    {
        var visitDate = new DateOnly(2024, 6, 15);
        var visit = new Visit
        {
            Description = "Dental cleaning",
            VisitDate = visitDate,
            PetId = 3
        };

        await _sut.CreateAsync(visit);

        var saved = await _db.Visits.FindAsync(visit.Id);
        Assert.NotNull(saved);
        Assert.Equal("Dental cleaning", saved.Description);
        Assert.Equal(visitDate, saved.VisitDate);
        Assert.Equal(3, saved.PetId);
    }

    [Fact]
    public async Task CreateAsync_CanStoreMultipleVisitsForSamePet()
    {
        var visit1 = new Visit { Description = "First visit", PetId = 1 };
        var visit2 = new Visit { Description = "Second visit", PetId = 1 };

        await _sut.CreateAsync(visit1);
        await _sut.CreateAsync(visit2);

        var visits = await _db.Visits.Where(v => v.PetId == 1).ToListAsync();
        Assert.Equal(2, visits.Count);
    }
}
