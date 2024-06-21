using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request
{
    public class DeletePermissionDtoRequest
    {
        public int Id { get; set; }
        public string User { get; set; }
    }
}
