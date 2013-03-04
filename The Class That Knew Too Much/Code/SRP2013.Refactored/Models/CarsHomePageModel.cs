using System.Collections.Generic;

namespace SRPTalk.Models
{
    public class CarsHomePageModel
    {
        public List<string> Makes { get; set; }
        public List<string> BodyTypes { get; set; }
        public List<string> Dealerships { get; set; }

        public IEnumerable<string> Log { get; set; }
    }
}