using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrdering.Models.ViewModels
{
    public class RatingViewModel
    {
        public string Title { get; set; }
        public List<Ratings> ListOfRatings { get; set; }
        public string Comments { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
    }
}
