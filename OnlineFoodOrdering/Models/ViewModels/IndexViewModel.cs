using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models.ViewModels
{
    public class IndexViewModel
    {
        public WishList WishList { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public Compare Compare { get; set; }
        public IEnumerable<MenuItem> MenuItem { get; set; }
        public IEnumerable<MenuItem> MenuItemFeatured { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }

        //public RatingViewModel RatingVM { get; set; }

    }
}
