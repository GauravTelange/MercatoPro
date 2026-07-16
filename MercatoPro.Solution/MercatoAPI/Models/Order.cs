using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MercatoAPI.Models
{
    public class Order
    {
            [Key]
            public int OrderId { get; set; }

            public int UserId { get; set; }
            public User User { get; set; }

            [Column(TypeName = "decimal(10,2)")]
            public decimal TotalAmount { get; set; }

            public string OrderStatus { get; set; } = "Pending";

            public DateTime OrderDate { get; set; } = DateTime.Now;

            public List<OrderItem> OrderItems { get; set; }
        
    }
}

