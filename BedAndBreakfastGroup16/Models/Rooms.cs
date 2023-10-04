using System.ComponentModel.DataAnnotations;

namespace BedAndBreakfastGroup16.Models
{
    public class Rooms
    {
        [Key]
        public int RoomId { get; set; }
        
        public string RoomType { get; set; }

        public string RoomImage { get; set; }

        public decimal RoomPrice { get; set; }

        public string RoomSize { get; set; }

        public int RoomCapacity { get; set; }

        public string RoomDescription { get; set;}
    }
}
