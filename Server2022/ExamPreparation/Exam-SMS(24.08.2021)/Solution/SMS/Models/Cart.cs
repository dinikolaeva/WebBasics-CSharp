using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Cart
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        IEnumerable<Product> Products { get; set; } = new HashSet<Product>();
    }
}
