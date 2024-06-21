using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Request
{
    public class CountryDtoRequest
    {
        public string CodeNum { get; set; }
        public int CountryId { get; set; }
        public string CodeTxt { get; set; }
        public string CodeLanguage { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
    }
}
