using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string CardId { get; set; }

        public Cart Cart { get; set; }
    }
}
