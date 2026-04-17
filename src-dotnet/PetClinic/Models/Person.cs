using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models;

public abstract class Person : BaseEntity
{
    [Required, StringLength(30), Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(30), Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
}
