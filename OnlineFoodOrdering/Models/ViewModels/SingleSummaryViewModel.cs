using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models.ViewModels
{
    public class SingleSummaryViewModel
    {
        public MenuItem MenuItem { get; set; }
        public Order Order { get; set; }
        public int Count { get; set; }
    }
}
