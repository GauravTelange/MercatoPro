using System.ComponentModel.DataAnnotations;

namespace AdminApp.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
