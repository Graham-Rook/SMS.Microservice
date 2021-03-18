using System.Collections.Generic;

namespace SMS.Microservice.Service.Helpers.LogHelper
{
    public class Profile
    {
        public string Name { get; set; }
        public List<Destination> Destinations { get; set; }


        public Profile()
        {
            if (string.IsNullOrEmpty(Name))
                Name = "Default";

            Destinations = new List<Destination>();
        }
    }
}
