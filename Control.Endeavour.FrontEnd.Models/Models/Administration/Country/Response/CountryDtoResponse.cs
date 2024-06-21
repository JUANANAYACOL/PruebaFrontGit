using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response
{
    public class CountryDtoResponse
    {
        public int CountryId { get; set; }
        public string CodeNum { get; set; }
        public string CodeTxt { get; set; }
        public string Name { get; set; }
        public string CodeLanguage { get; set; }
    }
}
