using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.State.Request
{
    public class StateDtoRequest
    {
        public int CountryId { get; set; }
        public string CodeNum { get; set; }
        public string CodeTxt { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public int StateId { get; set; }
    }
}
