using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models;

public class Owner : Person
{
    [Required, StringLength(255)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string City { get; set; } = string.Empty;

    [Required, RegularExpression(@"\d{10}", ErrorMessage = "Telephone must be a 10-digit number")]
    public string Telephone { get; set; } = string.Empty;

    public List<Pet> Pets { get; set; } = new();
}
