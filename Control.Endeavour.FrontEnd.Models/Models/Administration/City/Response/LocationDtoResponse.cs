using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response
{
    public class LocationDtoResponse
    {
        public string CountryCode { get; set; } = null!;

        public string Country { get; set; } = null!;

        public int StateId { get; set; }

        public string StateCode { get; set; } = null!;

        public string State { get; set; } = null!;

        public int CityId { get; set; }

        public string CityCode { get; set; } = null!;

        public string City { get; set; } = null!;
    }
}
