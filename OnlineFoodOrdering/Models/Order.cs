﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OrderTotalOriginal { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name ="Order Total")]
        public double OrderTotal { get; set; }
        [Required]
        [Display(Name ="PickUp Time")]
        public DateTime PickUpTime { get; set; }
        [Required]
        [NotMapped]
        public DateTime PickUpDate { get; set; }

        [Display(Name ="Coupon Code")]
        public string CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; } 
        public string Comments { get; set; }
        
        public string PickupName { get; set; }
        public string PickupLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string TransactionId { get; set; }
    }
}
