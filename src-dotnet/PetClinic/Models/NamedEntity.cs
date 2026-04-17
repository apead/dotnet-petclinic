using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models;

public abstract class NamedEntity : BaseEntity
{
    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;
}
