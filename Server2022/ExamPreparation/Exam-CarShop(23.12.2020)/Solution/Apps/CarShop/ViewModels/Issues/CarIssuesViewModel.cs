using CarShop.ViewModels.Issues;
using System.Collections.Generic;

namespace CarShop.ViewModels.Cars
{
    public class CarIssuesViewModel
    {
        public string Id { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public bool UserIsMechanic { get; set; }

        public IEnumerable<CarAddIssuesViewModel> Issues { get; set; }
    }
}
