using System.ComponentModel.DataAnnotations;

namespace BedAndBreakfastGroup16.Models
{
    public class BookingInformation
    {
        public string BookingId { get; set; }

        [Key]
        public int BookInfoID { get; set; }

        public int RoomType { get; set; } 

        public decimal RoomPrice { get; set; }

        public string BookingCustName { get; set; }

        public string BookingContactEmail { get; set; }

        public string BookingContactNumber { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int pax { get; set; }

        public string Status { get; set; }  // Pending, Approved, Declined
    }
}
