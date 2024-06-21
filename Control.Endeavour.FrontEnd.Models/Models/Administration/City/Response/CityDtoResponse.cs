using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response
{
    public class CityDtoResponse
    {
        public int CityId { get; set; }
        public string CodeNum { get; set; }
        public string CodeTxt { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
    }
}
