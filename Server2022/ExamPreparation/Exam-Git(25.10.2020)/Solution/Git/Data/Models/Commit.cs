using System;
using System.ComponentModel.DataAnnotations;

namespace Git.Data.Models
{
    public class Commit
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string CreatorId { get; set; }
        public User Creator { get; set; }
        public string RepositoryId { get; set; }
        public Repository Repository { get; set; }
    }
}
