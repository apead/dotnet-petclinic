using PetClinic.Models;
using Microsoft.EntityFrameworkCore;

namespace PetClinic.Data;

public static class DbInitializer
{
    public static void Initialize(PetClinicContext context)
    {
        // No EF migrations are used in this demo app. Deleting the database ensures
        // the schema is always up-to-date when the model changes.
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (context.Vets.Any()) return;

        var radiology = new Specialty { Name = "radiology" };
        var surgery = new Specialty { Name = "surgery" };
        var dentistry = new Specialty { Name = "dentistry" };
        context.Specialties.AddRange(radiology, surgery, dentistry);

        var vets = new[]
        {
            new Vet { FirstName = "James", LastName = "Carter" },
            new Vet { FirstName = "Helen", LastName = "Leary", Specialties = [radiology] },
            new Vet { FirstName = "Linda", LastName = "Douglas", Specialties = [surgery, dentistry] },
            new Vet { FirstName = "Rafael", LastName = "Ortega", Specialties = [surgery] },
            new Vet { FirstName = "Henry", LastName = "Stevens", Specialties = [radiology] },
            new Vet { FirstName = "Sharon", LastName = "Jenkins" },
        };
        context.Vets.AddRange(vets);

        var cat = new PetType { Name = "cat" };
        var dog = new PetType { Name = "dog" };
        var lizard = new PetType { Name = "lizard" };
        var snake = new PetType { Name = "snake" };
        var bird = new PetType { Name = "bird" };
        var hamster = new PetType { Name = "hamster" };
        context.PetTypes.AddRange(cat, dog, lizard, snake, bird, hamster);

        var owners = new[]
        {
            new Owner { FirstName = "Sipho", LastName = "Dlamini", Address = "12 Berea Rd.", City = "Durban", Telephone = "0315551023",
                Pets = [new Pet { Name = "Leo", BirthDate = new DateOnly(2010, 9, 7), Type = cat }] },
            new Owner { FirstName = "Zanele", LastName = "Mokoena", Address = "45 Church St.", City = "Pretoria", Telephone = "0125551749",
                Pets = [new Pet { Name = "Basil", BirthDate = new DateOnly(2012, 8, 6), Type = hamster }] },
            new Owner { FirstName = "Thabo", LastName = "Nkosi", Address = "88 Commissioner St.", City = "Johannesburg", Telephone = "0115558763",
                Pets = [
                    new Pet { Name = "Rosy", BirthDate = new DateOnly(2011, 4, 17), Type = dog },
                    new Pet { Name = "Jewel", BirthDate = new DateOnly(2010, 3, 7), Type = dog }
                ] },
            new Owner { FirstName = "Lerato", LastName = "Sithole", Address = "23 Adderley St.", City = "Cape Town", Telephone = "0215553198",
                Pets = [new Pet { Name = "Iggy", BirthDate = new DateOnly(2010, 11, 30), Type = lizard }] },
            new Owner { FirstName = "Pieter", LastName = "van der Merwe", Address = "7 Voortrekker Rd.", City = "Stellenbosch", Telephone = "0215552765",
                Pets = [new Pet { Name = "George", BirthDate = new DateOnly(2010, 1, 20), Type = snake }] },
            new Owner { FirstName = "Nomsa", LastName = "Khumalo", Address = "56 Florida Rd.", City = "Durban", Telephone = "0315552654",
                Pets = [
                    new Pet { Name = "Samantha", BirthDate = new DateOnly(2012, 9, 4), Type = cat,
                        Visits = [
                            new Visit { VisitDate = new DateOnly(2013, 1, 1), Description = "rabies shot" },
                            new Visit { VisitDate = new DateOnly(2013, 1, 4), Description = "spayed" }
                        ] },
                    new Pet { Name = "Max", BirthDate = new DateOnly(2012, 9, 4), Type = cat,
                        Visits = [
                            new Visit { VisitDate = new DateOnly(2013, 1, 2), Description = "rabies shot" },
                            new Visit { VisitDate = new DateOnly(2013, 1, 3), Description = "neutered" }
                        ] }
                ] },
            new Owner { FirstName = "Ruan", LastName = "Botha", Address = "14 Long St.", City = "Cape Town", Telephone = "0215555387",
                Pets = [new Pet { Name = "Lucky", BirthDate = new DateOnly(2011, 8, 6), Type = bird }] },
            new Owner { FirstName = "Ayanda", LastName = "Zulu", Address = "3 Sandton Dr.", City = "Sandton", Telephone = "0115557683",
                Pets = [new Pet { Name = "Mulligan", BirthDate = new DateOnly(2007, 2, 24), Type = dog }] },
            new Owner { FirstName = "Christiaan", LastName = "Joubert", Address = "99 Hatfield St.", City = "Pretoria", Telephone = "0125559435",
                Pets = [new Pet { Name = "Freddy", BirthDate = new DateOnly(2010, 3, 9), Type = bird }] },
            new Owner { FirstName = "Naledi", LastName = "Motsepe", Address = "31 West St.", City = "Johannesburg", Telephone = "0115555487",
                Pets = [
                    new Pet { Name = "Lucky", BirthDate = new DateOnly(2010, 6, 24), Type = dog },
                    new Pet { Name = "Sly", BirthDate = new DateOnly(2012, 6, 8), Type = cat }
                ] },
        };
        context.Owners.AddRange(owners);
        context.SaveChanges();
    }
}
