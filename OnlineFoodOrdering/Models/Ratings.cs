using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models
{
    public class Ratings
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        
        public int MenuItemId { get; set; }
        [Required]
        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
        [Required]
        public int Rating { get; set; }
        public string Comments { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    }
}
