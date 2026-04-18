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
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
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
            PetId = 1,
            VisitDate = new DateOnly(2025, 6, 15),
            Description = "Annual checkup"
        };

        await _sut.CreateAsync(visit);

        var saved = await _db.Visits.SingleAsync();
        Assert.Equal(1, saved.PetId);
        Assert.Equal(new DateOnly(2025, 6, 15), saved.VisitDate);
        Assert.Equal("Annual checkup", saved.Description);
    }

    [Fact]
    public async Task CreateAsync_AssignsIdToVisit()
    {
        var visit = new Visit
        {
            PetId = 2,
            VisitDate = new DateOnly(2025, 7, 1),
            Description = "Vaccination"
        };

        await _sut.CreateAsync(visit);

        Assert.True(visit.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleVisits_AllVisitsArePersisted()
    {
        var visit1 = new Visit { PetId = 1, VisitDate = new DateOnly(2025, 1, 10), Description = "Checkup" };
        var visit2 = new Visit { PetId = 2, VisitDate = new DateOnly(2025, 2, 20), Description = "Grooming" };

        await _sut.CreateAsync(visit1);
        await _sut.CreateAsync(visit2);

        Assert.Equal(2, await _db.Visits.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_WithVetId_PersistsVetAssociation()
    {
        var visit = new Visit
        {
            PetId = 3,
            VetId = 5,
            VisitDate = new DateOnly(2025, 3, 5),
            Description = "Post-surgery follow-up"
        };

        await _sut.CreateAsync(visit);

        var saved = await _db.Visits.SingleAsync();
        Assert.Equal(5, saved.VetId);
    }

    [Fact]
    public async Task CreateAsync_WithoutVetId_VetIdIsNull()
    {
        var visit = new Visit
        {
            PetId = 4,
            VisitDate = new DateOnly(2025, 4, 10),
            Description = "Routine exam"
        };

        await _sut.CreateAsync(visit);

        var saved = await _db.Visits.SingleAsync();
        Assert.Null(saved.VetId);
    }
}
