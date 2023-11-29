namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// 1. Implemente as models da aplicação
public class Hotel {
    [Key]
    public int HotelId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int CityId { get; set; }
    [ForeignKey("CityId")]
    public virtual City? City { get; set; }
    public virtual IEnumerable<Room>? Rooms { get; set; }
}