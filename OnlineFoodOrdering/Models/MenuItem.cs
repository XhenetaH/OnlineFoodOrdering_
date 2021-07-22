using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = ("The coupon name is too long."))]

        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = ("The coupon description is too long."))]

        public string Description { get; set; }
        public string Spicyness { get; set; }
        public enum ESpicy { NA=0, NotSpicy=1, Spicy=2, VerySpicy=3}
        public string Image { get; set; }
        [Display(Name="Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Display(Name="SubCategory")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
        [Range(1,int.MaxValue, ErrorMessage ="Price should be greater than ${1}")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public bool isFeatured { get; set; }
        public DateTime InsertDate { get; set; }
        public ICollection<Ratings> Ratings { get; set; }
    }
}
