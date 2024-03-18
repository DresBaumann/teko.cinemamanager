using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopMovie.CinemaManager.Core.Features.Cinemas.Models
{
    public class SeatIdentifier
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CinemaHall")]
        public int CinemaHallId { get; set; }

        [Required]
        [StringLength(5)]
        public string Row { get; set; }

        public int Number { get; set; }

        public bool IsAvailable { get; set; } = true;
        
        public CinemaHall CinemaHall { get; set; }
    }
}