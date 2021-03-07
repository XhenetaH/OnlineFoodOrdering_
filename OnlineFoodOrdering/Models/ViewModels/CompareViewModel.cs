using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models.ViewModels
{
    public class CompareViewModel
    {
        public List<Compare> CompareList { get; set; }
        public Compare Compare { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
      
    }
}
