using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(15,ErrorMessage =("The coupon name is too long."))]
        public string Name { get; set; }
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Percent=0, Euro=1 }
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]        
        public double Discount { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]      
        public double MinimumAmount { get; set; }
        public bool IsActive { get; set; }



    }
}
