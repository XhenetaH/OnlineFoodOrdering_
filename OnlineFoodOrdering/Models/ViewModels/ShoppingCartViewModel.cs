using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public IEnumerable<MenuItem> MenuItemList { get; set; }
        public IEnumerable<Ratings> Ratings { get; set; }
        public Ratings Rating { get; set; }

        public WishList WishList { get; set; }
    }
}
