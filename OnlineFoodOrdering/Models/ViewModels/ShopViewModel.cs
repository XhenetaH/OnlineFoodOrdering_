using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models.ViewModels
{
    public class ShopViewModel
    {
        public IEnumerable<MenuItem> MenuItem { get; set; }
        public IEnumerable<Category> Category { get; set; }        
        public int? CategoryId { get; set; }
        public IEnumerable<MenuItem> MenuItemFeatured { get; set; }
        public WishList WishList { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public Compare Compare { get; set; }
    }
}
