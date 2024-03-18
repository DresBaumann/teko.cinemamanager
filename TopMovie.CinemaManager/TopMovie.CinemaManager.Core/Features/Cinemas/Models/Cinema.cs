using System.ComponentModel.DataAnnotations;

namespace TopMovie.CinemaManager.Core.Features.Cinemas.Models;

public class Cinema
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string StreetName { get; set; }

    [Required]
    public int StreetNumber { get; set; }

    [Required]
    [RegularExpression(@"([1-468][0-9]|[57][0-7]|9[0-6])[0-9]{2}", ErrorMessage = "Invalid format.")]
    public string PLZ { get; set; }

    public ICollection<CinemaHall> CinemaHalls { get; set; } = new List<CinemaHall>();
}