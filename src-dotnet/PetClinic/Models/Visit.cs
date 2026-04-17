using System.ComponentModel.DataAnnotations;

namespace PetClinic.Models;

public class Visit : BaseEntity
{
    [Required, Display(Name = "Visit Date"), DataType(DataType.Date)]
    public DateOnly VisitDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required, StringLength(255)]
    public string Description { get; set; } = string.Empty;

    public int PetId { get; set; }
}
