using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request
{
    public class LocationDtoRequest : PaginationRequest
    {
        public string? Name { get; set; }

        public int CountryId { get; set; }

        public string? CountryCode { get; set; }

        public string? Country { get; set; }

        public int StateId { get; set; }

        public string? StateCode { get; set; }

        public string? State { get; set; }

        public int CityId { get; set; }

        public string? CityCode { get; set; }

        public string? City { get; set; }
    }
}
