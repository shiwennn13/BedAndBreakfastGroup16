using System.ComponentModel.DataAnnotations;

namespace BedAndBreakfastGroup16.Models
{
    public class BookingInformation
    {
        [Key]
        public int BookingId { get; set; }

        public int RoomId { get; set; }  // Foreign key to the Rooms table

        public string UserId { get; set; }  // Assuming you're using Identity, this would be the foreign key to the User table
        public DateTime BookingDate { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int pax { get; set; }

        public string Status { get; set; }  // Pending, Approved, Declined
    }
}
