namespace PetClinic.Models;

public class Vet : Person
{
    public List<Specialty> Specialties { get; set; } = new();

    public string SpecialtiesDisplay => Specialties.Count == 0
        ? "none"
        : string.Join(", ", Specialties.Select(s => s.Name));
}
