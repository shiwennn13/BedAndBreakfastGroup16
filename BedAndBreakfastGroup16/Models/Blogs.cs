using System.ComponentModel.DataAnnotations;

namespace BedAndBreakfastGroup16.Models
{
    public class Blogs
    {
        [Key]
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogType { get; set; }
        public string BlogImage { get; set; }
        public string BlogDescription { get; set; }

    }
}
