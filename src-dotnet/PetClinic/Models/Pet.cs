using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models;

public class Pet : NamedEntity
{
    [Required, Display(Name = "Birth Date"), DataType(DataType.Date)]
    public DateOnly BirthDate { get; set; }

    [Required]
    public int TypeId { get; set; }
    public PetType? Type { get; set; }

    public int OwnerId { get; set; }

    public List<Visit> Visits { get; set; } = new();
}
