using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request
{
    public class SecurityMangmemtParemeterDtoRequest
    {
        public int AppKeyId { get; set; }

        public string Value1 { get; set; } = null!;
    }
}